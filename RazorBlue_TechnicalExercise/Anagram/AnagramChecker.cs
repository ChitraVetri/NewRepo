using System;
using System.Linq;

namespace Anagram
{
    internal class AnagramChecker
    {
        static void Main(string[] args)
        {
            Console.Write("Enter 1st String : ");
            string input1 = Console.ReadLine();
            Console.Write("Enter 2nd String : ");
            string input2 = Console.ReadLine();
            bool areAnagrams = AnagramChecker.AreAnagrams(input1, input2);

            if (areAnagrams)
            {
                Console.WriteLine("The strings are anagrams.");
            }
            else
            {
                Console.WriteLine("The strings are not anagrams.");
            }
            Console.ReadLine();
        }
        public static bool AreAnagrams(string str1, string str2)
        {
            // Convert both strings to lowercase and remove any non-letter characters
            string cleanStr1 = new string(str1.ToLower().Where(char.IsLetter).ToArray());
            string cleanStr2 = new string(str2.ToLower().Where(char.IsLetter).ToArray());

            // Check if the lengths of the cleaned strings are equal
            if (cleanStr1.Length != cleanStr2.Length)
            {
                return false;
            }

            // Convert the cleaned strings to character arrays
            char[] chars1 = cleanStr1.ToCharArray();
            char[] chars2 = cleanStr2.ToCharArray();

            // Sort the character arrays
            Array.Sort(chars1);
            Array.Sort(chars2);

            // Compare the sorted character arrays
            for (int i = 0; i < chars1.Length; i++)
            {
                if (chars1[i] != chars2[i])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
