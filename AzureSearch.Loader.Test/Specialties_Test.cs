using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AzureSearch.Common;
using System;

namespace AzureSearch.Loader.Test
{
    [TestClass]
    public class Specialties_Test
    {
        [TestMethod]
        public void GetAllSpecialitiesAliasesAndTypes_Test01()
        {
            List<Provider> providers = new List<Provider>();
            int index;
            Alias a1 = new Alias();
            Alias a2 = new Alias();
            Alias a3 = new Alias();
            Alias a4 = new Alias();
            Alias a5 = new Alias();
            Alias a6 = new Alias();
            Alias a7 = new Alias();
            Alias a8 = new Alias();
            Specialty s1 = new Specialty();
            Specialty s2 = new Specialty();
            Specialty s3 = new Specialty();
            Specialty s4 = new Specialty();
            List<Specialty> sList1;
            List<Specialty> sList2;
            Provider p1 = new Provider();
            Provider p2 = new Provider();
            Provider p3 = new Provider();
            List<SpecialtyAliasAndType> specialtiesAliasesAndTypes;

            //Empty provider list.
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(0, specialtiesAliasesAndTypes.Count);

            //Single provider with null components.
            providers.Add(p1);
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(0, specialtiesAliasesAndTypes.Count);

            //Single provider with empty specialties collection.
            p1.specialties = new Specialty[0];
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(0, specialtiesAliasesAndTypes.Count);

            //Single provider with single item in specialties collection.  Only a specialty.  No aliases.
            s1.aliases = null;
            s1.specialty = "specialty01";
            s1.subspecialty = null;
            sList1 = new List<Specialty>() { s1 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(1, specialtiesAliasesAndTypes.Count);
            //One alias
            index = 0;
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[0].Alias);
            Assert.AreEqual(null, specialtiesAliasesAndTypes[0].Specialty);
            Assert.AreEqual(EntryTypes.Specialty, specialtiesAliasesAndTypes[0].EntryType);

            //Single provider with single item in specialties collection.  One specialty with one alias.
            s1.specialty = "specialty01";
            s1.subspecialty = null;
            a1 = new Alias { name = "a1" };
            s1.aliases = new Alias[1] { a1 };
            sList1 = new List<Specialty>() { s1 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(1, specialtiesAliasesAndTypes.Count);
            //One alias
            index = 0;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);

            //Single provider with single item in specialties collection.  One specialty with no aliases but a subspecialty.
            s1.specialty = "specialty01";
            s1.subspecialty = "subs";
            s1.aliases = null;
            sList1 = new List<Specialty>() { s1 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(1, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("subs", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);

            //Single provider with single item in specialties collection.  One specialty with two similar aliases and a subspecialty.
            s1.specialty = "specialty01";
            s1.subspecialty = "subs";
            a1 = new Alias { name = "a1" };
            a2 = new Alias { name = "a1" };
            s1.aliases = new Alias[2] { a1, a2 };
            sList1 = new List<Specialty>() { s1 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(3, specialtiesAliasesAndTypes.Count);

            index = 0;
            Assert.AreEqual("subs", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);

            //Single provider with single item in specialties collection.  One specialty with two dissimilar aliases.
            s1.specialty = "specialty01";
            s1.subspecialty = null;
            a1 = new Alias { name = "a1" };
            a2 = new Alias { name = "a2" };
            s1.aliases = new Alias[2] { a1, a2 };
            sList1 = new List<Specialty>() { s1 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(2, specialtiesAliasesAndTypes.Count);
            //Two aliases
            index = 0;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);

            //Single provider with single item in specialties collection.  One specialty with two dissimilar aliases plus same subspecialty.
            s1.specialty = "specialty01";
            s1.subspecialty = "specialty01";
            a1 = new Alias { name = "a1" };
            a2 = new Alias { name = "a2" };
            s1.aliases = new Alias[2] { a1, a2 };
            sList1 = new List<Specialty>() { s1 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);
            Assert.AreEqual(2, specialtiesAliasesAndTypes.Count);
            //Two aliases
            index = 0;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);

            //Single provider with multiple specialties.
            s1.specialty = "specialty01";
            s1.subspecialty = "sub1";
            a1 = new Alias { name = "a1" };
            a2 = new Alias { name = "a2" };
            s1.aliases = new Alias[2] { a1, a2 };

            s2.specialty = "specialty02";
            s2.subspecialty = "sub2";
            a3 = new Alias { name = "a3" };
            a4 = new Alias { name = "a4" };
            s2.aliases = new Alias[2] { a3, a4 };
            sList1 = new List<Specialty>() { s1, s2 };
            p1.specialties = sList1.ToArray();
            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);

            Assert.AreEqual(6, specialtiesAliasesAndTypes.Count);

            index = 0;
            Assert.AreEqual("sub1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("sub2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty02", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a3", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty02", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a4", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty02", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);

            //Multiple providers with multiple specialties.
            s1.specialty = "specialty01";
            s1.subspecialty = "sub1";
            a1 = new Alias { name = "a1" };
            a2 = new Alias { name = "a2" };
            s1.aliases = new Alias[2] { a1, a2 };

            s2.specialty = "specialty02";
            s2.subspecialty = "sub2";
            a3 = new Alias { name = "a3" };
            a4 = new Alias { name = "a4" };
            s2.aliases = new Alias[2] { a3, a4 };
            sList1 = new List<Specialty>() { s1, s2 };
            p1.specialties = sList1.ToArray();

            s3.specialty = "specialty03";
            s3.subspecialty = "sub3";
            a5 = new Alias { name = "a5" };
            a6 = new Alias { name = "a6" };
            s3.aliases = new Alias[2] { a5, a6 };

            s4.specialty = "specialty04";
            s4.subspecialty = "sub4";
            a7 = new Alias { name = "a7" };
            a8 = new Alias { name = "a8" };
            s4.aliases = new Alias[2] { a7, a8 };
            sList2 = new List<Specialty>() { s3, s4 };
            p2 = new Provider();
            p2.specialties = sList2.ToArray();
            providers.Add(p2);

            specialtiesAliasesAndTypes = Specialties.AccumulateAllSpecialitiesAliasesAndTypes(providers);

            Assert.AreEqual(12, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("sub1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty01", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("sub2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty02", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a3", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty02", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a4", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty02", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("sub3", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty03", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a5", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty03", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a6", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty03", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("sub4", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty04", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a7", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty04", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            index++;
            Assert.AreEqual("a8", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual("specialty04", specialtiesAliasesAndTypes[index].Specialty);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);

        }

        [TestMethod]
        public void Dedupe_Test01()
        {
            List<SpecialtyAliasAndType> specialtiesAliasesAndTypes = new List<SpecialtyAliasAndType>();
            int index;

            //Dedupe empty list
            specialtiesAliasesAndTypes = Specialties.DeDupe(specialtiesAliasesAndTypes);
            Assert.AreEqual(0, specialtiesAliasesAndTypes.Count);

            //Dedupe one entry list
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "sp1",
                EntryType = EntryTypes.Specialty,
                Specialty = null
            });
            specialtiesAliasesAndTypes = Specialties.DeDupe(specialtiesAliasesAndTypes);
            Assert.AreEqual(1, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Specialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual(null, specialtiesAliasesAndTypes[index].Specialty);

            //Dedupe three entry list
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "a2",
                EntryType = EntryTypes.Alias,
                Specialty = "sp2"
            });
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "su1",
                EntryType = EntryTypes.Subspecialty,
                Specialty = "sp2"
            });
            specialtiesAliasesAndTypes = Specialties.DeDupe(specialtiesAliasesAndTypes);
            Assert.AreEqual(3, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Specialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual(null, specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp2", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("su1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp2", specialtiesAliasesAndTypes[index].Specialty);

            //Dedupe multiple entry list
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "a2",
                EntryType = EntryTypes.Alias,
                Specialty = "sp2"
            });
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "su1",
                EntryType = EntryTypes.Subspecialty,
                Specialty = "sp2"
            });
            specialtiesAliasesAndTypes = Specialties.DeDupe(specialtiesAliasesAndTypes);
            Assert.AreEqual(3, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Specialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual(null, specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp2", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("su1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp2", specialtiesAliasesAndTypes[index].Specialty);

        }
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void Dedupe_Test02()
        {
            List<SpecialtyAliasAndType> specialtiesAliasesAndTypes = new List<SpecialtyAliasAndType>();
            //Dedupe invalid entry list
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "a1",
                EntryType = EntryTypes.Subspecialty,
                Specialty = "sp1"
            });
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "a1",
                EntryType = EntryTypes.Alias,
                Specialty = "sp1"
            });
            specialtiesAliasesAndTypes = Specialties.DeDupe(specialtiesAliasesAndTypes);

        }
        [TestMethod]
        public void RemoveSpecialtyEntriesWithAliases_Test01()
        {
            List<SpecialtyAliasAndType> specialtiesAliasesAndTypes = new List<SpecialtyAliasAndType>();
            int index;

            //No specialty entries
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "a1",
                EntryType = EntryTypes.Alias,
                Specialty = "sp1"
            });
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "a2",
                EntryType = EntryTypes.Alias,
                Specialty = "sp1"
            });
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "su1",
                EntryType = EntryTypes.Subspecialty,
                Specialty = "sp1"
            });

            specialtiesAliasesAndTypes = Specialties.RemoveSpecialtyEntriesWithAliases(specialtiesAliasesAndTypes);

            Assert.AreEqual(3, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("su1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);

            //Specialty entry on its own.
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "sp2",
                EntryType = EntryTypes.Specialty,
                Specialty = null
            });

            specialtiesAliasesAndTypes = Specialties.RemoveSpecialtyEntriesWithAliases(specialtiesAliasesAndTypes);

            Assert.AreEqual(4, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("su1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("sp2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Specialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual(null, specialtiesAliasesAndTypes[index].Specialty);

            //Specialty entry with one or more aliases.
            specialtiesAliasesAndTypes.Add(new SpecialtyAliasAndType
            {
                Alias = "sp1",
                EntryType = EntryTypes.Specialty,
                Specialty = null
            });

            specialtiesAliasesAndTypes = Specialties.RemoveSpecialtyEntriesWithAliases(specialtiesAliasesAndTypes);

            Assert.AreEqual(4, specialtiesAliasesAndTypes.Count);
            index = 0;
            Assert.AreEqual("a1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("a2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Alias, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("su1", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Subspecialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual("sp1", specialtiesAliasesAndTypes[index].Specialty);
            index++;
            Assert.AreEqual("sp2", specialtiesAliasesAndTypes[index].Alias);
            Assert.AreEqual(EntryTypes.Specialty, specialtiesAliasesAndTypes[index].EntryType);
            Assert.AreEqual(null, specialtiesAliasesAndTypes[index].Specialty);

        }
    }
}