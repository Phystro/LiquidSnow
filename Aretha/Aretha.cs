﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Thismaker.Aretha
{
    static class Aretha
    {
        public static async Task Prepare(string[] args)
        {
            Console.ResetColor();
            if (args != null && args.Length > 0)
            {
                await Act(args);
                return;
            }

            Speak("Ready to summon");

            var instruction = Listen();

            if (instruction == null) return;

            await Act(instruction.Split(' '));
        }

        static async Task Act(string[] args)
        {
            var left_args = new List<string>();

            for(int i = 2; i < args.Length; i++)
            {
                left_args.Add(args[i]);
            }

            if (args[0] == "summon")
            {
                if (args[1] == "anubis")
                {
                    await AnubisSoul.Summon("Anubis has been called",left_args.ToArray());
                }
                if (args[1] == "enigma")
                {
                    await EnigmaSoul.Summon("Anubis has been called",left_args.ToArray());
                }
                else
                {
                    Speak("Command not recognized");
                }
            }
            else
            {
                Speak("Command not recognized");
            }

            await Prepare(null);
        }

        public static void SoulFailed(Soul soul, Exception ex)
        {
            Speak($"{soul} was unable to executed your command: {ex.Message}");
        }

        public static ConsoleColor SoulColor(Soul soul)
        {
            return soul switch
            {
                Soul.Aretha => ConsoleColor.Yellow,
                Soul.Anubis => ConsoleColor.Cyan,
                Soul.Enigma=>ConsoleColor.Green,
                _=>ConsoleColor.White
            };
        }

        public static void Speak(string text, Soul soul= Soul.Aretha)
        {
            Console.ForegroundColor = SoulColor(soul);
            Console.Write($"{soul}: ");
            Console.ResetColor();
            Console.Write(text);
            Console.WriteLine();
        }

        public static string Listen(bool isYN = false, Soul soul=Soul.Aretha)
        {
            Console.ForegroundColor = SoulColor(soul);
            Console.Write(">> ");
            Console.ResetColor();
            var instruction = Console.ReadLine().ToLower();

            if (instruction == "@aretha exit")
            {
                Environment.Exit(0);
            }

            if (instruction == "@aretha dismiss")
            {
                return null;
            }

            if (isYN)
            {
                if (instruction != "y" && instruction != "n")
                {
                    Speak("This response requires a Y (For Yes) or N (For No). Try again.");
                    return Listen(isYN);
                }
            }

            return instruction;
        }

        public static string ListenCS(bool isYN = false, Soul soul = Soul.Aretha)
        {
            Console.ForegroundColor = SoulColor(soul);
            Console.Write(">> ");
            Console.ResetColor();
            var instruction = Console.ReadLine();

            if (instruction.ToLower() == "@aretha exit")
            {
                Environment.Exit(0);
            }

            if (instruction.ToLower() == "@aretha dismiss")
            {
                return null;
            }

            if (isYN)
            {
                if (instruction.ToLower() != "y" && instruction.ToLower() != "n")
                {
                    Speak("This response requires a Y (For Yes) or N (For No). Try again.");
                    return Listen(isYN);
                }
            }

            return instruction;
        }

        public static string GetPath(string text, bool input, bool confirm = true, Soul soul=Soul.Aretha)
        {
            Speak(text, soul);
            var response = ListenCS(false, soul);
            if (response == null) return null;

            var path = response.Trim();
            path = path.Trim('\"');
            if (confirm)
            {
                if (input && !File.Exists(path))
                {
                    Speak("The file you have provided does not exist!",soul);
                    return GetPath(text, input, confirm, soul);
                }
                else if (!input && File.Exists(path))
                {
                    Speak("This file already exists. Are you sure you want to overwrite it? (Y/N)", soul);
                    response = Listen(true, soul);
                    if (response == null) return null;

                    if (response != "y") return GetPath(text, input, confirm, soul);
                }
            }
            return path;
        }

    }

    enum Soul {Aretha, Anubis, Enigma }

}