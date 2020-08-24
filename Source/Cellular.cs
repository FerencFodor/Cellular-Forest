using System;
using System.Collections.Generic;
using Mono.Options;

namespace CellularAutomaton.Source
{
    class Cellular
    {

        struct Probability
        {
            public int tree1;
            public int tree2;
            public int empty;
        };

        struct Name
        {
            public char tree1;
            public char tree2;
            public char empty;
        };

        private static readonly Random random = new Random();


        static Probability probability;
        static Name name;
        static int size;
        //public static readonly int size = 60;

        static int Main(string[] args)
        {
            size = 60;
            name = new Name { tree1 = 'T', tree2 = 'P', empty = ' ' };
            probability = new Probability { tree1 = 20, tree2 = 30, empty = 50 };
            bool show_help = false;

            OptionSet option = new OptionSet()
            {
                {"s|size=", "size of the grid",
                (int v) => size = v},

                {"p1", "probalility of first item.",
                    (int v) => probability.tree1 = v},
                {"p2", "probalility of second item.",
                    (int v) => probability.tree2 = v},
                {"pe", "probalility of empty item.",
                    (int v) => probability.empty = v},

                {"h|help", "show this message and exit",
                    v => show_help = v != null}
            };

            

            try
            {
                option.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e);
                return -1;
            }

            if (show_help)
            {
                ShowHelp(option);
                return 0;
            }

            Console.Clear();

            string table = "";
            bool continueFlag = true;

            for (int i = 0; i < (size * size); i++)
            {
                table += PseudoRandomCharacterGenerator();
            }

            do
            {
                Console.Clear();
                PrintTable(table);
                table = CellularAutomaton(table);
                if (Char.ToUpper(GetKeyPress("Refine (Y/N) ", new Char[] { 'Y', 'N' })) == 'N')
                    continueFlag = false;


            } while (continueFlag);

            return 0;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: greet [OPTIONS]+ message");
            Console.WriteLine("Greet a list of individuals with an optional message.");
            Console.WriteLine("If no message is specified, a generic greeting is used.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        public static string CellularAutomaton(string block)
        {
            string generation = "";

            for(int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    int neightbour = 0;
                    int tree1Neighbour = 0;
                    int tree2Neighbour = 0;


                    //Find Neighbour
                    for (int xOffset = -1; xOffset < 2; xOffset++)
                    {
                        for(int yOffset = -1; yOffset < 2; yOffset++)
                        {
                            int newX = x + xOffset;
                            int newY = y + yOffset;

                            if ((newX < 0 || newY < 0) || (newX >= size || newY >= size))
                                continue;
                            if (newX == 0 && newY == 0)
                                continue;
                            if (block[newX * size + newY] != name.empty)
                            {
                                neightbour++;
                                if (block[newX * size + newY] == name.tree1) tree1Neighbour++;
                                else if (block[newX * size + newY] == name.tree2) tree2Neighbour++;
                            }
                                
                        }
                    }

                    //Check rules
                    if(block[x * size + y] != name.empty)
                    {
                        if(neightbour < 4)
                        {
                            generation += name.empty;
                        }else
                        {
                            generation += block[x * size + y];
                        }
                    }
                    else if (block[x * size + y] == name.empty)
                    {
                        if(neightbour >= 5)
                        {
                            generation += tree1Neighbour > tree2Neighbour ? name.tree1 : name.tree2;
                        } else
                        {
                            generation += block[x * size + y];
                        }
                    }
                }
            }

            return generation;
        }

        public static int maxValue(params int[] probabilities)
        {
            int maxValue = 0;
            foreach(int value in probabilities)
                maxValue += value;

            return maxValue;
        }

        public static void PrintTable(string table)
        {
            string output = "";

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    output += table[i * size + j] + " ";
                }

                output += '\n';
            }

            Console.WriteLine(output);
        }

        public static char PseudoRandomCharacterGenerator()
        {
            int value = random.Next(maxValue(probability.tree1, probability.tree2, probability.empty)) + 1;

            if(value > probability.tree1)
            {
                value -= probability.tree1;
                if(value > probability.tree2)
                {
                    return name.empty;
                }else
                {
                    return name.tree2;
                }
            }
            else
            {
                return name.tree1;
            }

            
        }

        private static Char GetKeyPress(String msg, Char[] validChars)
        {
            ConsoleKeyInfo keyPressed;
            bool valid = false;

            Console.WriteLine();
            do
            {
                Console.Write(msg);
                keyPressed = Console.ReadKey();
                Console.WriteLine();
                if (Array.Exists(validChars, ch => ch.Equals(Char.ToUpper(keyPressed.KeyChar))))
                    valid = true;
            } while (!valid);
            return keyPressed.KeyChar;
        }
    }
}
