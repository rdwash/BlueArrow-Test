using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueArrowTest.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var dictionary = new string[] { "cold", "wind", "snow", "chill" };
            var srcMatrix = new string[] { "abcdc", "fgwio", "chill", "pqnsd", "uvdxy" };

            var results = new WordFinder(dictionary: dictionary).Find(src: srcMatrix);

            if (results.Any())
            {
                foreach (string result in results)
                {
                    System.Console.Write(result + " ");
                }
            }
            else
                System.Console.Write("No words found from the dictionary.");

            System.Console.ReadKey();          
        }
    }

    public class WordFinder : IWordFinder
    {
        public Dictionary<string, bool> _foundWords;
        public Dictionary<string, bool> FoundWords
        {
            get { return _foundWords ?? new Dictionary<string, bool>(StringComparer.Ordinal); }
            set { _foundWords = value; }
        }

        private IEnumerable<string> _dictionary;
        public IEnumerable<string> Dictionary
        {
            get { return _dictionary ?? new string[] { }; }
            set { _dictionary = value; }
        }

        private char[,] _matrix;
        public char[,] Matrix
        {
            get { return _matrix ?? new char[,] { }; }
            set { _matrix = value; }
        }

        public WordFinder(IEnumerable<string> dictionary)
        {
            Dictionary = dictionary ?? Dictionary;
            FoundWords = new Dictionary<string, bool>(StringComparer.Ordinal);
        }

        public IList<string> Find(IEnumerable<string> src)
        {
            if (src == null)
                return new List<string>();

            //Build Matrix            
            BuildMatrix(src);
            
            var numberOfMatrixCols = Matrix.GetLength(1);
            var numberOfMatrixRows = Matrix.GetLength(0);           

            // Going to iterate over each position in the matrix.
            for (int colPos = 0; colPos < numberOfMatrixCols; colPos++)
            {
                for (int rowPos = 0; rowPos < numberOfMatrixRows; rowPos++)
                {
                    // Search all directions, not just horizontally and vertically, but diagonally as well
                    for (int d = 0; d < 8; d++)
                    {
                        SearchPostion(Matrix, colPos, rowPos, numberOfMatrixCols, numberOfMatrixRows, "", d);
                    }
                }
            }

            var resultList = new List<string>();
            if (FoundWords.Any())
            {
                foreach (KeyValuePair<string, bool> word in FoundWords)
                {
                    resultList.Add(word.Key);
                }
            }

            return resultList;
        }

        private void BuildMatrix(IEnumerable<string> src)
        {
            var numberOfMatrixRows = src.Count();
            var numberOfMatrixCols = src.First().Length;

            Matrix = new char[numberOfMatrixRows, numberOfMatrixCols];

            for (var i = 0; i < numberOfMatrixRows; i++)
            {
                for (var j = 0; j < numberOfMatrixCols; j++)
                {
                    Matrix[i, j] = src.ToArray()[i].ToCharArray()[j];
                }
            }
        }

        private void SearchPostion(char[,] matrix, int colPosition, int rowPosition, int numberOfCols, int numberOfRows, string build, int direction)
        {
            // Array bounds check.
            if (colPosition >= numberOfCols 
                || colPosition < 0 
                || rowPosition >= numberOfRows 
                || rowPosition < 0)
            {
                return;
            }

            // Get letter.
            char letter = matrix[rowPosition, colPosition];

            // Append.
            string wordToSearchOn = build + letter;

            // Add a word to "FoundWords" dictionary object if, 
            // as we're building a search string from the matrix position values (our switch statement below), 
            // we find a match against in our dictionary of words.
            // Skip dups.
            if (!FoundWords.ContainsKey(wordToSearchOn))
            {
                if (Dictionary.Any(x => x == wordToSearchOn))
                {
                    FoundWords.Add(wordToSearchOn, true);
                    return;
                }
            }

            //Keep searching based on direction value: E(0), S(1), SE(2), W(3), N(4), NW(5), SW(6), NE(7)
            //Advance the search position via Recursion
            switch (direction)
            {
                //East
                case 0:
                    SearchPostion(matrix, colPosition + 1, rowPosition, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //South
                case 1:
                    SearchPostion(matrix, colPosition, rowPosition + 1, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //Southeast
                case 2:
                    SearchPostion(matrix, colPosition + 1, rowPosition + 1, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //West
                case 3:
                    SearchPostion(matrix, colPosition - 1, rowPosition, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //North
                case 4:
                    SearchPostion(matrix, colPosition, rowPosition - 1, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //Northwest
                case 5:
                    SearchPostion(matrix, colPosition - 1, rowPosition - 1, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //Southwest
                case 6:
                    SearchPostion(matrix, colPosition - 1, rowPosition + 1, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
                //Northeast
                case 7:
                    SearchPostion(matrix, colPosition + 1, rowPosition - 1, numberOfCols, numberOfRows, wordToSearchOn, direction);
                    break;
            }                    
        }
    }

    public interface IWordFinder
    {
        IList<string> Find(IEnumerable<string> src);
    }   
}