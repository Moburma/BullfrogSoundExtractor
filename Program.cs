using System;
using System.IO;
using System.Text;

class Program
{
    static int tEndCounter; // End of samples table counter
    static Boolean sWars;
    static Boolean MC1;
    static Boolean MC2;
    
        static byte[] SubArray(byte[] array, int start, int length)
    {
        byte[] result = new byte[length];
        Array.Copy(array, start, result, 0, length);
        return result;
    }

    static void Main(string[] args)
    {
        string inputFile = "sound.dat"; // Default input file
        string formatType;
        string hiPath;
        string loPath;
        int numbTables;

        // Check for command-line arguments
        if (args.Length > 0)
        {
            // Check for input file argument
            if (File.Exists(args[0]))
            {
                inputFile = args[0];
            }
            else
            {
                Console.WriteLine($"Input file '{args[0]}' not found. Falling back to default file 'sound.dat'.");
                //sWars = true; //set to Syndicate Wars default mode
            }

            // Check for format type argument
            if (args.Length > 1)
            {
                formatType = args[1].ToLower(); // Convert to lowercase for case-insensitive comparison

                if (formatType == "sw")
                {
                    sWars = true;
                }
                else if (formatType == "gw")
                {
                    sWars = true;
                }
                else if (formatType == "mc1")
                {
                    MC1 = true;
                }
                else if (formatType == "mc2")
                {
                    MC2 = true;
                }
                else if (formatType == "th")
                {
                    sWars = false;
                }
                else if (formatType == "dk")
                {
                    sWars = false;
                }
                
            }
        }
        else
        {
            Console.WriteLine("Bullfrog Sound Extractor version 0.1 by Moburma");
            Console.WriteLine("");
            Console.WriteLine("Arguments: BullFrogSoundExtractor.exe [Input file] [Game]");
            Console.WriteLine("");
            Console.WriteLine("e.g. BullFrogSoundExtractor.exe SOUND.DAT swars");
            Console.WriteLine("");
            Console.WriteLine("Supported games:");
            Console.WriteLine("dk - Dungeon Keeper");
            Console.WriteLine("gw - Gene Wars");
            Console.WriteLine("mc1 - Magic Carpet");
            Console.WriteLine("mc2 - Magic Carpet 2: The NetherWorlds");
            Console.WriteLine("th - Theme Hospital");
            Console.WriteLine("sw - Syndicate Wars");
            Console.WriteLine("");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine("Error - SOUND.DAT file not found. You must run this program from a directory with sound.dat present!");
            Console.WriteLine();
            return; 
        }

        Console.WriteLine("Starting");

        byte[] soundFile = File.ReadAllBytes(inputFile);
        int tStart = soundFile.Length - 4;
        //Console.WriteLine($"Samples table is at {tStart}");

        if (sWars) { 
        hiPath = "Hi";
        loPath = "Lo";
        //Console.WriteLine("SW Mode");
        Directory.CreateDirectory(hiPath);
        Directory.CreateDirectory(loPath);
        }
        else {
            hiPath = "extracted";
            loPath = "";
            Directory.CreateDirectory(hiPath);
        }

        int fPos = Convert32BitInt(soundFile[tStart], soundFile[tStart + 1], soundFile[tStart + 2], soundFile[tStart + 3]);

        if (MC1)
        {
            numbTables = soundFile[fPos + 12]; // number of tables/soundbanks in file. MC1/2, Gene Wars and Syndicate Wars Japanese use more than 1

        }
        else if (MC2)
        {
            numbTables = soundFile[fPos + 6]; // number of tables/soundbanks in file. MC1/2, Gene Wars and Syndicate Wars Japanese use more than 1

        }
        else
        {
            numbTables = soundFile[fPos + 4]; // number of tables/soundbanks in file. MC1/2, Gene Wars and Syndicate Wars Japanese use more than 1
        }
        Console.WriteLine($"Number of sample tables: {numbTables}");

        bool first = true;
        if (File.Exists("filelist.txt"))
        {
            File.Delete("filelist.txt");
        }

            do
        {
            if (first)
            {
                if (MC1)
                {
                    fPos += 114; // Move to the first sample bank
                }
                else if (MC2)
                {
                    fPos += 60; // Move to the first sample bank
                }
                else
                {
                    fPos += 50; // Move to the first sample bank
                }
            }
            else if (sWars)
            {
                fPos += 80; // Move to the second and subsequent sample banks
            }
            else if (MC1)
            {
                fPos += 144; // Move to the second and subsequent sample banks
            }
            else if (MC2)
            {
                fPos += 96; // Move to the second and subsequent sample banks
            }

            int tPos = Convert32BitInt(soundFile[fPos], soundFile[fPos + 1], soundFile[fPos + 2], soundFile[fPos + 3]);
            //Console.WriteLine($"Data start is {tPos}");

            int offset = Convert32BitInt(soundFile[fPos + 4], soundFile[fPos + 5], soundFile[fPos + 6], soundFile[fPos + 7]);
            int tEnd = Convert32BitInt(soundFile[fPos + 8], soundFile[fPos + 9], soundFile[fPos + 10], soundFile[fPos + 11]);

            tPos += 32; // First entry is blank
            //Console.WriteLine($"End is {tEnd}");

            tEndCounter = 32;

            string hiOutputPath = Path.Combine(hiPath, "");
            string loOutputPath = Path.Combine(loPath, "");

            Console.WriteLine("Extracting High quality sounds");

            ExtractSounds(soundFile, tPos, offset, tEnd, hiOutputPath);

            //Console.WriteLine(offset);
            if (sWars)
            {
                fPos += 64; // Move to the low quality sample table if SWars

                tPos = Convert32BitInt(soundFile[fPos], soundFile[fPos + 1], soundFile[fPos + 2], soundFile[fPos + 3]);
                //Console.WriteLine($"Data start is {tPos}");

                offset = Convert32BitInt(soundFile[fPos + 4], soundFile[fPos + 5], soundFile[fPos + 6], soundFile[fPos + 7]);
                tEnd = Convert32BitInt(soundFile[fPos + 8], soundFile[fPos + 9], soundFile[fPos + 10], soundFile[fPos + 11]);

                //Console.WriteLine($"End is {tEnd}");

                tPos += 32; // First entry is blank
                tEndCounter = 32;

                Console.WriteLine("Extracting Low quality sounds");

                ExtractSounds(soundFile, tPos, offset, tEnd, loOutputPath);
            }
            numbTables--;
            first = false;

        } while (numbTables > 0);
    }

    static void ExtractSounds(byte[] soundFile, int tPos, int offset, int tEnd, string outputPath)
    {
        do
        {
            byte[] fNameBytes = new byte[18];
            Array.Copy(soundFile, tPos, fNameBytes, 0, 18);
            string fName = Encoding.UTF8.GetString(fNameBytes).Trim('\0');
            Console.WriteLine(fName);

            int fSegment1 = Convert32BitInt(soundFile[tPos + 18], soundFile[tPos + 19], soundFile[tPos + 20], soundFile[tPos + 21]);
            //Console.WriteLine($"Start position {fSegment1}");

            fSegment1 += offset;
            //Console.WriteLine($"Real start position {fSegment1}");

            int fLength = Convert32BitInt(soundFile[tPos + 26], soundFile[tPos + 27], soundFile[tPos + 28], soundFile[tPos + 29]);
            //Console.WriteLine($"File length {fLength}");

            int fSegment2 = fSegment1 + fLength;
            //Console.WriteLine(fSegment2);

            string oFile = Path.Combine(outputPath, fName);
            File.WriteAllBytes(oFile, SubArray(soundFile, fSegment1, fSegment2 - fSegment1));
            //Console.WriteLine($"Writing file {fName}");
            File.AppendAllText("filelist.txt", fName + Environment.NewLine);

            tPos += 32;
            tEndCounter = tEndCounter + 32;
            //Console.WriteLine($"tendcounter is {tEndCounter} and tend is {tEnd}");
        } while (tEndCounter < tEnd);
    }

    static int Convert32BitInt(byte byteOne, byte byteTwo, byte byteThree, byte byteFour)
    {
        byte[] convertBytes32 = { byteOne, byteTwo, byteThree, byteFour };
        return (int)BitConverter.ToUInt32(convertBytes32, 0);
    }
}
