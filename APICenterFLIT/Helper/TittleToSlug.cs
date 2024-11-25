using System.Globalization;
using System.Text;

namespace APICenterFlit.Helper
{
    public class TittleToSlug
    {
        public static string ConvertToSlug(string input)
        {
            input = input.ToLower();

            input = RemoveDiacritics(input); // Bo dau TV

            input = input.Replace(" ", "-");

            return input;
        }
        public static string RemoveDiacritics(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
