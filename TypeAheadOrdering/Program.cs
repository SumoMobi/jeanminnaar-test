using System;
using System.Collections.Generic;
using TypeAheadOrdering.DataModels;
using System.Linq;

namespace TypeAheadOrdering
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DataModels.TypeAheadItem> typeAheadItems = PopulateList(); 
            //We received the results back from the SP and converted the JSON into a List of a known entity (TypeAheadItem).
            //Assuming we do not have meaningless words in the search terms at this time and neither characters like commas and so on.

            //term the user keyed in:
            string term = "Ortho S";

            //Order Specialty
            List<string> specialtiesOrdered = OrderItems(typeAheadItems
                .Where(t => t.Category == "Specialty")
                .Select(t => t.Suggestion)
                .ToList(), term);

            Console.WriteLine("Hit any key to close");
            Console.ReadKey();

        }

        private static List<string> OrderItems(List<string> suggestionsToOrder, string term)
        {
            if (suggestionsToOrder == null || suggestionsToOrder.Count == 0)
            {
                return suggestionsToOrder;
            }

            //User can provide multiple search words.  Each word has to match something in the suggestions.  
            //If the user provided a multi-word search, the results will only contain suggestions where each provided word matches at least
            //one word in the suggestion.  Stated differently, the suggestions will have at least as many words as the user provided.
            //Weights are assigned as follows:
            //  First word matches first word in suggestion: 1000
            //  
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] terms = term.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

            //Get Maximum number of words we have for a suggestion.
            int maxSuggestionWords = 0;
            foreach(string suggestion in suggestionsToOrder)
            {
                int count = suggestion.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries).Length;
                if (count > maxSuggestionWords)
                {
                    maxSuggestionWords = count;
                }
            }

            List<SuggestionPlusWeight> suggestionsPlusWeights = suggestionsToOrder
                .Select(s => new SuggestionPlusWeight
                {
                    Suggestion = s,
                    Weight = 0
                })
                .ToList();

            for (int t = 0; t < terms.Length; t++)
            {
                int weight = terms.Length - t - 1;  //Weight of the search term word.

                for (int i = 0; i < suggestionsPlusWeights.Count; i++)
                {
                    string[] suggestionWords = suggestionsPlusWeights[i].Suggestion.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    for (int w = 0; w < suggestionWords.Length; w++)
                    {
                        int factor = maxSuggestionWords - w + 1;
                        if (suggestionWords[w].StartsWith(terms[t]))    //Should use case insensitive method.
                        {
                            suggestionsPlusWeights[i].Weight += (int)Math.Pow(weight + factor, 2);
                        }
                    }
                }
            }

            suggestionsPlusWeights = suggestionsPlusWeights.OrderByDescending(l => l.Weight).ThenBy(l => l.Suggestion).ToList();

            Console.WriteLine($"term {term}");
            foreach(SuggestionPlusWeight t in suggestionsPlusWeights)
            {
                Console.WriteLine($"{t.Suggestion:50}: {t.Weight}");
            }
            //return the list
            return suggestionsPlusWeights.Select(w => w.Suggestion).ToList();
        }

        private static List<TypeAheadItem> PopulateList()
        {
            List<TypeAheadItem> items = new List<TypeAheadItem>();
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Elbow Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Foot Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Foot and Ankle Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Hip Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Hip and Knee Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Knee Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Non-Surgical Orthopedics" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Orthopedic Oncology" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Orthopedic Sports Medicine" });
            items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Orthopedic Surgery" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopnea" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthostasis" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthotics" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopedic infections" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopedic stem-cell regeneration" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopedic tumor" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthostatic hypotension" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthostatic proteinuria" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "postural orthostatic tachycardia syndrome" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "chronic pain after orthopedic surgery" });
            items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "Orthopedic HVN Provider" });
            return items;
        }
    }
}
