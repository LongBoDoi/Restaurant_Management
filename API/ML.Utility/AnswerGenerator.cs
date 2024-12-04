using Mysqlx.Session;
using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace API.ML.Utility
{
    public class AnswerGenerator
    {
        public static List<string> GetPossibleAnswers(string input)
        {
            List<string> results = new List<string>();

            if (input.Contains(" / "))
            {
                var everyInputs = input.Split(" / ");
                foreach (var eachInput in everyInputs)
                {
                    GenerateCombinationsHelper(eachInput, "", results);
                }
            } else
            {
                GenerateCombinationsHelper(input, "", results);
            }

            return results;
        }

        static void GenerateCombinationsHelper(string input, string current, List<string> results)
        {
            if (string.IsNullOrEmpty(input))
            {
                results.Add(current.Trim());
                return;
            }

            // Regex to match the first set of parentheses or alternatives
            Match match = Regex.Match(input, @"\(([^()]*(\([^()]*\))*[^()]*)\)|([^()\s]+/[^\s()]+)");

            if (!match.Success)
            {
                var everyInput = input.Split('/');
                foreach (var eachInput in everyInput)
                {
                    results.Add($"{current}{eachInput}".Trim());
                }

                return;
            }

            // Before the match
            string beforeMatch = input.Substring(0, match.Index);
            current += beforeMatch;

            if (match.Value.StartsWith("("))
            {
                // Optional part
                string optionalPart = match.Groups[1].Value;

                GenerateCombinationsHelper(optionalPart + input.Substring(match.Index + match.Length), current, results);

                // Also consider the case without the optional part
                GenerateCombinationsHelper(input.Substring(match.Index + match.Length), current, results);
            }
            else
            {
                // Alternatives part
                string[] alternatives = match.Value.Split('/');
                foreach (string alternative in alternatives)
                {
                    GenerateCombinationsHelper(input.Substring(match.Index + match.Length), current + alternative, results);
                }
            }
        }

        /// <summary>
        /// Lấy dữ liệu đáp án
        /// </summary>
        /// <param name="panel">Panel chứa đáp án</param>
        /// <returns></returns>
        public async static Task<Dictionary<int, string>> GetAnswerData(IElementHandle panel)
        {
            Dictionary<int, string> dicAnswers = [];

            if (panel != null)
            {
                string strAnswerData = await panel.EvaluateFunctionAsync<string>("el => el.textContent");

                string pattern = @"(\d+)\s*(?:The correct answer is\s*)?([a-zA-Z])";

                MatchCollection matches = Regex.Matches(strAnswerData, pattern);

                foreach (Match match in matches)
                {
                    dicAnswers.Add(int.Parse(match.Groups[1].Value), match.Groups[2].Value);
                }
            }

            return dicAnswers;
        }
    }
}
