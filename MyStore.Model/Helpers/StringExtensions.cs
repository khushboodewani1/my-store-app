namespace MyStore.Model.Helpers
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var stringBuilder = new System.Text.StringBuilder();

            // Loop through each character in the input string.
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                // If the character is uppercase, convert it to lowercase and insert an underscore before it (unless it's the first character).
                if (char.IsUpper(ch))
                {
                    if (i > 0)// If it's not the first character, add an underscore before the lowercase letter.
                        stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLower(ch));
                }
                else
                {
                    // If the character is already lowercase, just append it.
                    stringBuilder.Append(ch);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
