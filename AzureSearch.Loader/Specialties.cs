using AzureSearch.Common;
using AzureSearch.CosmosDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureSearch.Loader
{
    public class SpecialtyAliasAndType
    {
        public string Specialty { get; set; }
        public string Alias { get; set; }
        public EntryTypes EntryType { get; set; }
    }
    public class SpecialtyAliasAndTypeComparer : IEqualityComparer<SpecialtyAliasAndType>
        {
        public bool Equals(SpecialtyAliasAndType left, SpecialtyAliasAndType right)
        {
            if (left.Alias != right.Alias)
            {
                return false;
            }
            //Aliases are the same.  Same specialty?
            if (left.Specialty == null && right.Specialty == null)
            {
                return true;
            }

            if (left.Specialty != right.Specialty)
            {
                return false;
            }

            if (left.EntryType != right.EntryType)
            {   //We can have subspecialty and alias entries with the same "alias" value.  Just keep one of the two.
                if (left.EntryType == EntryTypes.Specialty || right.EntryType == EntryTypes.Specialty)
                {
                    throw new ApplicationException("Two SpecialtyAliasAndType entries have same alias and specialty but different entry types." +
                        $"alias {left.Alias}, specialty {left.Specialty}, left entry type {left.EntryType.ToString()}, right entry type {right.EntryType.ToString()}");
                }
                return true;
            }
            return true;
        }

        public int GetHashCode(SpecialtyAliasAndType obj)
        {
            int hash = 0;
            for (int c = 0; c < obj.Alias.Length; c++)
            {
                hash += obj.Alias[c];
            }
            return hash;
        }
    }
    public enum EntryTypes { Specialty, Alias, Subspecialty }

    public class Specialties
    {
        public static void Upload(string apiKey, string serviceName)
        {
            DateTime startDateTime = DateTime.Now;

            //Get all the provider data.
            List<KyruusDataStructure> providers = ProviderDa.GetAllFromDownload();
            Console.WriteLine($"{providers.Count} providers fetched.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Get the complete list of aliases and specialties.
            startDateTime = DateTime.Now;
            List<SpecialtyAliasAndType> specialtiesAliasesAndTypes = AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Console.WriteLine($"{specialtiesAliasesAndTypes.Count} specialtiesAliasesAndTypes.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //De-dupe the list and remove specialties for where we have aliases.
            startDateTime = DateTime.Now;
            specialtiesAliasesAndTypes = DeDupe(specialtiesAliasesAndTypes);
            Console.WriteLine($"{specialtiesAliasesAndTypes.Count} de-duped.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            startDateTime = DateTime.Now;
            specialtiesAliasesAndTypes = RemoveSpecialtyEntriesWithAliases(specialtiesAliasesAndTypes);
            Console.WriteLine($"{specialtiesAliasesAndTypes.Count} entries after removing some specialty entries.  Response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Drop and recreate the Azure Index for suggestions.
            startDateTime = DateTime.Now;
            RecreateSuggestionIndexes();
            Console.WriteLine($"Recreate suggestion indexes response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

            //Populate specialties index.
            startDateTime = DateTime.Now;
            SpecialtyIndexDataStructure[] specialties = new SpecialtyIndexDataStructure[specialtiesAliasesAndTypes.Count];
            for (int i = 0; i < specialtiesAliasesAndTypes.Count; i++)
            {
                specialties[i] = new SpecialtyIndexDataStructure
                {
                    alias = specialtiesAliasesAndTypes[i].Alias,
                    id = (i + 1).ToString(),
                    specialty = specialtiesAliasesAndTypes[i].Specialty
                };
            }
            IndexLoader.MergeOrUpload(specialties.ToList(), apiKey, serviceName, "specialties");
            Console.WriteLine($"PopulateSpecialtiesIndex response time {(DateTime.Now - startDateTime).TotalMilliseconds}");

        }

        private static void RecreateSuggestionIndexes()
        {
            Console.WriteLine("First need to drop and create the Specialties index.  Hit ENTER when done.");
            Console.ReadLine();
            //Drop the indexes
            //TODO.  For now do it through Postman.
            //Create the indexes
            //TODO.  For now do it through Postman.
        }

        //If we end up with specialty entries but in fact there is one or more alias entries for the specialty, remove the specialty entry.
        //Another way of looking at it, is if we have an entry with an alias value but specialty is null, we have a specialty entry.
        //And if that same specialty is elsewhere with an alias or subspecialty, the specialty entry should be removed. 
        public static List<SpecialtyAliasAndType> RemoveSpecialtyEntriesWithAliases(List<SpecialtyAliasAndType> specialtiesAliasesAndTypes)
        {
            List<SpecialtyAliasAndType> specialtyTypeEntries = specialtiesAliasesAndTypes
                .Where(s => s.EntryType == EntryTypes.Specialty)
                .ToList();

            if (specialtyTypeEntries.Count == 0)
            {
                return specialtiesAliasesAndTypes;
            }

            List<SpecialtyAliasAndType> specialtyEntriesToRemove = new List<SpecialtyAliasAndType>();

            foreach (SpecialtyAliasAndType st in specialtyTypeEntries)
            {
                //See if there are aliases for the specialty entry.  The specialty field for the specialty type entries is null.
                if (specialtiesAliasesAndTypes.Exists(s => s.Specialty == st.Alias))
                {
                    specialtyEntriesToRemove.Add(st);
                }
            }
            
            if (specialtyEntriesToRemove.Count == 0)
            {
                return specialtiesAliasesAndTypes;
            }
            specialtiesAliasesAndTypes = specialtiesAliasesAndTypes
                .Except(specialtyEntriesToRemove, new SpecialtyAliasAndTypeComparer())
                .ToList();
            return specialtiesAliasesAndTypes;
        }
        public static List<SpecialtyAliasAndType> DeDupe(List<SpecialtyAliasAndType> specialtiesAliasesAndTypes)
        {
            List<SpecialtyAliasAndType> dedupeList = specialtiesAliasesAndTypes.Distinct(new SpecialtyAliasAndTypeComparer()).ToList();
            return dedupeList;
        }
        public static List<SpecialtyAliasAndType> AccumulateAllSpecialitiesAliasesAndTypes(List<KyruusDataStructure> providers)
        {
            List<SpecialtyAliasAndType> specialtiesAliasesAndTypes = new List<SpecialtyAliasAndType>();
            for (int i = 0; i < providers.Count; i++)
            {
                KyruusDataStructure p = providers[i];
                if (p.specialties == null)
                {   //This provider has no specialties.
                    continue;
                }
                foreach (Specialty s in p.specialties)
                {

                    if (string.IsNullOrWhiteSpace(s.subspecialty) && (s.aliases == null || s.aliases.Length == 0))
                    { //If the specialty has no aliases and no subspecialty, use specialty as alias/synonym.
                        specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
                        {
                            Alias = s.specialty,
                            EntryType = EntryTypes.Specialty,
                            Specialty = null
                        });
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(s.subspecialty) == false && s.specialty != s.subspecialty)
                    {   //Have a subspecialty that is different from specialty.  Treat subspecialty same as alias.
                        if (s.specialty != s.subspecialty)
                        {
                            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
                            {
                                Alias = s.subspecialty,
                                EntryType = EntryTypes.Subspecialty,
                                Specialty = s.specialty
                            });
                        }
                    }

                    if (s.aliases == null)
                    {
                        return specialtiesAliasesAndTypes;
                    }
                    foreach (Alias a in s.aliases)
                    {
                        if (s.specialty != a.name)
                        {
                            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
                            {
                                Alias = a.name,
                                EntryType = EntryTypes.Alias,
                                Specialty = s.specialty
                            });
                        }
                    }
                }
            }

            return specialtiesAliasesAndTypes;
        }
    }
}
