namespace SynacorChallenge
{
    public class Teleporter
    {
        public static ushort Ackermann(int h, int m, int n)
        {
            var previous = new ushort[0x8000];
            var current = new ushort[0x8000];

            for (var i = 0; i <= m; i++)
            {
                for (var j = 0; j < 0x8000; j++)
                {
                    if (i == 0)
                    {
                        current[j] = (ushort) ((j + 1) & 0x7fff);
                    }
                    else if (j == 0)
                    {
                        current[j] = previous[h];
                    }
                    else
                    {
                        current[j] = previous[current[j - 1]];
                    }

                    if (i == m && j == n)
                    {
                        return current[n];
                    }
                }

                var temp = previous;
                previous = current;
                current = temp;
            }

            return 0;
        }
    }
}