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

            //Find the relevant suggestions for the user's search criteria.
            string searchTerm;
            List<string> suggestionsOrdered;
            List<string> relevantSuggestions;

            searchTerm = "fibroid";    //User's search criteria
            Console.WriteLine($"Search Criteria: {searchTerm}");
            Console.WriteLine("");
            relevantSuggestions = EmulateTheStoredProc(searchTerm);
            Console.WriteLine("Before sorting the returned suggestions");
            WriteSuggestions(relevantSuggestions);
            suggestionsOrdered = OrderSuggestions(relevantSuggestions, searchTerm);

            searchTerm = "uter fibroid";    //User's search criteria
            Console.WriteLine($"Search Criteria: {searchTerm}");
            Console.WriteLine("");
            relevantSuggestions = EmulateTheStoredProc(searchTerm);
            Console.WriteLine("Before sorting the returned suggestions");
            WriteSuggestions(relevantSuggestions);
            suggestionsOrdered = OrderSuggestions(relevantSuggestions, searchTerm);

            searchTerm = "vaginal i";    //User's search criteria
            Console.WriteLine($"Search Criteria: {searchTerm}");
            Console.WriteLine("");
            relevantSuggestions = EmulateTheStoredProc(searchTerm);
            Console.WriteLine("Before sorting the returned suggestions");
            WriteSuggestions(relevantSuggestions);
            suggestionsOrdered = OrderSuggestions(relevantSuggestions, searchTerm);

            Console.WriteLine("Hit any key to close");
            Console.ReadKey();

        }

        private static void WriteSuggestions(List<string> relevantSuggestions)
        {
            foreach (string suggestion in relevantSuggestions)
            {
                Console.WriteLine(suggestion);
            }
        }

        private static List<string> EmulateTheStoredProc(string searchTerm)
        {
            //User can provide multiple search words.  Each word has to match something in the suggestions.  
            //If the user provided a multi-word search, the results will only contain suggestions where each provided word matching at least
            //one word in the suggestion.  Stated differently, the suggestions will have at least as many words as the user provided.

            List<string> suggestions = PopulateList();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            List<string> searchWords = searchTerm
                .Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.ToLower())
                .Distinct()
                .ToList();

            List<string> suggestionResults = new List<string>();

            foreach (string suggestion in suggestions)
            {   //See which suggestions we want to show.  Which ones match the search criteria.
                List<string> suggestionWords = suggestion
                    .Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.ToLower())
                    .ToList();

                int foundCount = 0;
                foreach (string searchWord in searchWords)
                {   //See if each search word can be found in the suggestion.
                    foreach (string suggestionWord in suggestionWords)
                    { 
                        if (suggestionWord.StartsWith(searchWord))
                        {
                            foundCount++;
                            break;
                        }
                    }
                }
                if (foundCount == searchWords.Count)
                {
                    suggestionResults.Add(suggestion);
                }
            }
            return suggestionResults;
        }

        private static List<string> OrderSuggestions(List<string> suggestionsToOrder, string searchTerm)
        {
            if (suggestionsToOrder == null || suggestionsToOrder.Count == 0)
            {
                return suggestionsToOrder;
            }

            //Weights are assigned as follows:
            //  First word matches first word in suggestion: 1000
            //  
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] searchWords = searchTerm.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

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

            List<SuggestionPlusWeightAndWordCount> suggestionsPlusWeightsAndWordCount = suggestionsToOrder
                .Select(s => new SuggestionPlusWeightAndWordCount
                {
                    Suggestion = s,
                    Weight = 0
                })
                .ToList();

            for (int t = 0; t < searchWords.Length; t++)
            {
                int weight = searchWords.Length - t - 1;  //Weight of the search term word.

                for (int i = 0; i < suggestionsPlusWeightsAndWordCount.Count; i++)
                {
                    string[] suggestionWords = suggestionsPlusWeightsAndWordCount[i].Suggestion.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    suggestionsPlusWeightsAndWordCount[i].numberOfWords = suggestionWords.Length;
                    for (int w = 0; w < suggestionWords.Length; w++)
                    {
                        int factor = maxSuggestionWords - w + 1;
                        if (suggestionWords[w].StartsWith(searchWords[t]))    //Should use case insensitive method.
                        {
                            suggestionsPlusWeightsAndWordCount[i].Weight += (int)Math.Pow(weight + factor, 2);
                            //Console.WriteLine($"suggestion word: {w} {suggestionWords[w]}: searchWord: {t} {searchWords[t]} term weight: {weight} suggestionWord factor {factor}");
                            //Console.WriteLine($"{suggestionsPlusWeights[i].Suggestion:50}: {suggestionsPlusWeights[i].Weight}");
                        }
                    }
                }
            }

            suggestionsPlusWeightsAndWordCount = suggestionsPlusWeightsAndWordCount
                .OrderByDescending(l => l.Weight)
                .ThenBy(l => l.numberOfWords)
                .ThenBy(l => l.Suggestion).ToList();
            Console.WriteLine("");
            Console.WriteLine("After sorting the returned suggestions");
            foreach (SuggestionPlusWeightAndWordCount spw in suggestionsPlusWeightsAndWordCount)
            {
                Console.WriteLine($"{spw.Suggestion} {spw.Weight}");
            }
            Console.WriteLine("");
            return suggestionsPlusWeightsAndWordCount.Select(w => w.Suggestion).ToList();
        }

        private static List<string> PopulateList()
        {
            List<string> items = new List<string>() {
                "postmenopausal bleeding",
                "post menopausal bleeding",
                "bleeding after menopause",
                "surgical sterilization",
                "elective sterilization",
                "surgical birth control",
                "perinatal mood disorders",
                "vulvar abscess",
                "bartholins abscess",
                "vulvar disease",
                "vulvar atrophy",
                "vulvar abnormality",
                "vulvar dysplasia",
                "vulvar intraepithelial neoplasia",
                "vulvodynia",
                "vaginal pain",
                "vestibulitis",
                "vestibulodynia",
                "vulvar pain",
                "vulvar vestibulitis",
                "vaginitis",
                "recurrent vaginitis",
                "laceration of vagina",
                "vaginal laceration",
                "vaginal infections",
                "frequent vaginal infections",
                "uterine myoma",
                "uterine growth",
                "uterine myomas",
                "vaginal irritation",
                "vaginal itching",
                "anti-mullerian hormone testing",
                "anti-mullerian hormone measurement",
                "vaginismus",
                "vaginal laser therapy for vaginal atrophy",
                "mona lisa touch",
                "primary ovarian insufficiency",
                "uterine ablation",
                "minimally invasive hysteroscopy",
                "permanent sterilization",
                "mastodynia",
                "sore breast",
                "painful breast",
                "breast tenderness",
                "mastalgia",
                "discomfort in breast",
                "breast discomfort",
                "ache breast",
                "breast pain",
                "vaginal hysterectomy",
                "trans vaginal hysterectomy",
                "pelvic abscess",
                "birth control",
                "endometritis",
                "adenocarcinoma in situ of the cervix",
                "cervical carcinoma in situ",
                "cis of cervix",
                "carcinoma in situ of the cervix",
                "ais of cervix",
                "carcinoma in situ cervix",
                "cervical cancer in situ",
                "endometrioma",
                "postcoital bleeding",
                "bleeding after intercourse",
                "bleeding during intercourse",
                "fibroid removal",
                "uterine myomectomy",
                "myomectomy",
                "fibroids",
                "uterine fibroids",
                "submucous fibroid",
                "fibroid",
                "laparoscopic transabdominal cerclage",
                "birth control counseling",
                "bioidentical hormone replacement therapy",
                "bioidenticals",
                "vaginal delivery",
                "uterine surgery",
                "loop electrosurgical excision procedure",
                "uterine lining removal",
                "obgyn test",
                "ob test",
                "uterine polyp",
                "uterine fibroid embolization",
                "uterine fibroid embolization counseling",
                "reproductive health",
                "hgsil pap",
                "lgsil pap",
                "abnormal pap",
                "endometrial biopsy",
                "endometrial ablation",
                "prenatal diagnosis",
                "missed abortion",
                "missed periods",
                "missed period",
                "ectopic pregnancy",
                "tubal pregnancy",
                "female orgasmic disorder",
                "female anorgasmia",
                "menstrual disorders in athletes",
                "water birth",
                "annual gynecological examination",
                "annual gynecological exam",
                "pregnancy",
                "pregnant",
                "pregnancy counseling",
                "pelvic inflammatory disease",
                "pid",
                "vaginal bleeding after hysterectomy",
                "preconception counseling",
                "preconception counsel",
                "preconception consultation",
                "cystodynia",
                "cystalgia",
                "bladder pain",
                "uterine bleeding",
                "dysfunctional uterine bleeding",
                "abnormal uterine bleeding",
                "dub",
                "vaginal abscess",
                "postpartum care",
                "postpartum depression",
                "vaginal mesh implant complications",
                "mesh exposure",
                "complication of vaginal mesh",
                " mesh erosion",
                "women's health",
                "women's services",
                "womens services",
                "womens health",
                "female health",
                "in-office endometrial ablation",
                "stress incontinence",
                "urinary stress incontinence",
                "family planning",
                "operations on cul-de-sac",
                "female sexual dysfunction",
                "lichen sclerosus",
                "bxo",
                "balanitis xerotica obliterans",
                "lichen sclerosis",
                "transverse vaginal septum",
                "natural birth",
                "gonorrhea",
                "gonococcal infection",
                "laparoscopic myomectomy",
                "myomectomy (laparoscopic)",
                "nausea of pregnancy",
                "cancer risk reduction surgery",
                "partial hysterectomy",
                "postpartum thyroiditis",
                "single site surgery",
                "single site robotic surgery",
                "minimally invasive gynecologic surgery",
                "hysterectomy",
                "subtotal hysterectomy",
                "uterus removal",
                "chlamydia",
                "chlamydia std",
                "paratubal cyst",
                "cervical length assessment",
                "cancer-related sexual concerns",
                "abortion",
                "pregnancy termination",
                "pelviscopy",
                "dyspareunia",
                "painful intercourse",
                "perimenopause",
                "water labor",
                "hysteroscopic myomectomy",
                "mastitis",
                "breast infection",
                "menstrual migraines",
                "laparoscopic hysterectomy",
                "cervical funneling",
                "zika virus testing",
                "cervical dysplasia",
                "hysteroscopy",
                "pap smear",
                "annual pap",
                "bartholin's cyst",
                "bartholin gland cyst",
                "vulvar cyst",
                "intrauterine device",
                "iud removal",
                "iud placement",
                "iud insertion",
                "fetal ultrasound",
                "leiomyoma",
                "in-office sterilization",
                "spontaneous abortion",
                "sab",
                "pregnancy loss",
                "miscarriage",
                "genital herpes",
                "simplex genital herpes",
                "genital lesions",
                "breast examination",
                "breast exam",
                "breast self-exam",
                "breast self exam",
                "screening breast examination",
                "breast lump",
                "breast nodule",
                "lump or mass in breast",
                "lumpy breasts",
                "breast mass",
                "cystitis",
                "bladder inflammation",
                "bladder infections",
                "uti",
                "bladder infection",
                "acute cystitis",
                "cystitis cystica",
                "cystocele",
                "first trimester screening",
                "gynecologic disorders of athletes",
                "zika virus treatment",
                "prenatal testing",
                "pregnancy screening",
                "colposcopy",
                "dysuria",
                "pain with peeing",
                "painful urination",
                "premenstrual dysphoric disorder",
                "premenstrual syndrome",
                "pms",
                "cryosurgery of cervix",
                "incomplete emptying of bladder",
                "incomplete bladder emptying",
                "cervical insufficiency",
                "cervical polyp",
                "ovarian torsion",
                "ovarian cystectomy",
                "labial hypertrophy",
                "follicular cyst of ovary",
                "long acting reversible contraceptive",
                "larc",
                "prenatal ultrasound scan",
                "prenatal ultrasound",
                "pelvic organ prolapse",
                "genital prolapse",
                "pelvic prolapse ",
                "weakening of pubocervical tissue",
                "pelvic floor prolapse",
                "gynecological exam",
                "menstruation",
                "pelvic adhesions",
                "hormone replacement therapy",
                "hormone replacement",
                "hrt counseling",
                "hormone replacement therapy counseling",
                "hrt",
                "estrogen replacement",
                "hormone therapy",
                "estrogen",
                "robotic myomectomy",
                "myomectomy (robotic)",
                "hot flashes",
                "human papilloma virus",
                "high risk hpv",
                "high risk human papilloma virus",
                "hpv",
                "recurrent pregnancy loss",
                "hysteroscopic adhesiolysis",
                "hypertensive disorders of pregnancy",
                "dilation and curettage",
                "d&c",
                "dilatation and curettage",
                "suboxone treatment in pregnancy",
                "ovarian cyst",
                "ruptured ovarian cyst",
                "cyst on ovaries",
                "cysts on ovaries",
                "hemorrhagic ovarian cyst",
                "cyst on ovary",
                "lactation support",
                "irregular menstruation",
                "abnormal menstruation",
                "abnormal menstrual cycle",
                "abnormal periods",
                "irregular or no period",
                "irregular periods",
                "irregular menses",
                "menstrual abnormalities",
                "intermenstrual bleeding",
                "urinary incontinence",
                "bladder incontinence",
                "mixed urinary incontinence",
                "loss of control of urination",
                "bladder control problems",
                "incontinent of urine",
                "light bladder leakage",
                "urinary loss of control",
                "loss of bladder control",
                "weak bladder",
                "voiding incontinence",
                "incontinence of urine",
                "bladder dysfunction",
                "urine incontinence",
                "cholestasis of pregnancy",
                "menorrhagia",
                "heavy periods",
                "heavy bleeding",
                "heavy bleeding with periods",
                "uterine anomaly in pregnancy",
                "oophorectomy",
                "heavy menstrual bleeding",
                "vulvitis",
                "prenatal care",
                "amenorrhea",
                "teen pregnancy",
                "bacterial vaginosis",
                "bv",
                "sexually transmitted diseases",
                "std screening and management",
                "sexually transmitted infections",
                "sti's",
                "sexually transmitted disease",
                "std",
                "sti",
                "std signs",
                "cervicitis",
                "cesarean section",
                "pelvic varicose veins",
                "da vinci hysterectomy",
                "robotic davinci hysterectomy",
                "davinci hysterectomy",
                "robotic da vinci hysterectomy",
                "chronic pelvic pain",
                "female pelvic pain",
                "pelvic pain",
                "pelvic pain chronic",
                "severe pelvic pain",
                "pelvic pain management",
                "endometriosis",
                "ovarian endometriosis",
                "rectovaginal fistula",
                "recto-vaginal fistula",
                "rectocele",
                "vaginal rectocele",
                "weakening of rectovaginal tissue",
                "proctocele",
                "oligomenorrhea",
                "frequent urinary tract infections",
                "frequent urinary tract infection",
                "frequent uti",
                "abdominal cerclage",
                "transabdominal cerclage",
                "trans abdominal cerclage",
                "secondary amenorrhea",
                "laparoscopic salpingo oophorectomy",
                "bilateral salpingo-oophorectomy (laparoscopic)",
                "uterine abnormality",
                "adenomyosis",
                "uterine anomaly",
                "uterine thickening",
                "tubo-ovarian abscess",
                "tubal ligation",
                "tubal ligation reversal",
                "urge incontinence",
                "urge incontinence of urine",
                "urinary urge incontinence",
                "sexual dysfunction",
                "routine gynecological care",
                "routine gyn care",
                "adolescent gynecology",
                "threatened abortion",
                "labor and delivery",
                "labial adhesions",
                "anovulatory bleeding",
                "vaginal dryness",
                "dry vagina",
                "atypical glandular cells of undetermined significance",
                "agus pap",
                "pudendal neuralgia",
                "pudendal nerve entrapment",
                "pudendal neuropathy",
                "labial hypoplasia",
                "atrophic vaginitis",
                "vaginal biopsy",
                "vulva biopsy",
                "vaginal birth after caesarean section",
                "trial of labor after caesarean",
                "vbac",
                "atypical squamous cells of undetermined significance pap smear",
                "ascus pap smear",
                "menometrorrhagia",
                "heavy bleeding between periods",
                "heavy bleeding at irregular intervals",
                "breastfeeding",
                "breast feeding",
                "lactation disorders",
                "lactation" };
            return items;
        }
        //private static List<TypeAheadItem> PopulateList()
        //{
        //    List<TypeAheadItem> items = new List<TypeAheadItem>();
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Elbow Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Foot Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Foot and Ankle Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Hip Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Hip and Knee Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Knee Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Non-Surgical Orthopedics" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Orthopedic Oncology" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Orthopedic Sports Medicine" });
        //    items.Add(new TypeAheadItem { Category = "Specialty", Suggestion = "Orthopedic Surgery" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopnea" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthostasis" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthotics" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopedic infections" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopedic stem-cell regeneration" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthopedic tumor" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthostatic hypotension" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "orthostatic proteinuria" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "postural orthostatic tachycardia syndrome" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "chronic pain after orthopedic surgery" });
        //    items.Add(new TypeAheadItem { Category = "Condition", Suggestion = "Orthopedic HVN Provider" });
        //    return items;
        //}
        }
}
