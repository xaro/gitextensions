using System;
using System.Collections.Generic;

namespace GitCommands
{
    public class GitVersion : IComparable<GitVersion>
    {
        private static readonly GitVersion v1_7_0 = new GitVersion("1.7.0");
        private static readonly GitVersion v1_7_1 = new GitVersion("1.7.1");

        private const string Prefix = "git version";

        private readonly string full;
        private readonly int a;
        private readonly int b;
        private readonly int c;
        private readonly int d;

        public GitVersion(string version)
        {
            full = Fix(version);

            IList<int> numbers = GetNumbers(full);
            a = Get(numbers, 0);
            b = Get(numbers, 1);
            c = Get(numbers, 2);
            d = Get(numbers, 3);
        }

        public bool CloneCanAskForProgress
        {
            get { return this >= v1_7_0; }
        }

        public bool FetchCanAskForProgress
        {
            get { return this >= v1_7_1; }
        }

        public bool GuiDiffToolExist
        {
            get { return this >= v1_7_0; }
        }

        public bool IsUnknown
        {
            get { return a == 0 && b == 0 && c == 0 && d == 0; }
        }

        private static string Fix(string version)
        {
            if (version == null)
                return String.Empty;

            if (version.StartsWith(Prefix))
                return version.Substring(Prefix.Length).Trim();

            return version.Trim();
        }

        private static int Get(IList<int> values, int index)
        {
            return index < values.Count ? values[index] : 0;
        }

        private static IList<int> GetNumbers(string version)
        {
            IEnumerable<int> numbers = ParseNumbers(version);
            return new List<int>(numbers);
        }

        private static IEnumerable<int> ParseNumbers(string version)
        {
            string[] numbers = version.Split('.');

            foreach (var number in numbers)
            {
                int value;

                if (Int32.TryParse(number, out value))
                {
                    yield return value;
                }
            }
        }

        public int CompareTo(GitVersion other)
        {
            return Compare(this, other);
        }

        private static int Compare(GitVersion left, GitVersion right)
        {
            if (left == null && right == null) return 0;
            if (right == null) return 1;
            if (left == null) return -1;
            
            int compareA = left.a.CompareTo(right.a);
            if (compareA != 0) return compareA;

            int compareB = left.b.CompareTo(right.b);
            if (compareB != 0) return compareB;

            int compareC = left.c.CompareTo(right.c);
            if (compareC != 0) return compareC;

            return left.d.CompareTo(right.d);
        }

        public static bool operator >(GitVersion left, GitVersion right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator <(GitVersion left, GitVersion right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator >=(GitVersion left, GitVersion right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(GitVersion left, GitVersion right)
        {
            return Compare(left, right) <= 0;
        }

        public override string ToString()
        {
            return full;
        }
    }
}
