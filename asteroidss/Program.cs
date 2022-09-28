﻿using System;

namespace asteroidss
{

    class players
    {
        public int x = 5, y = 14; // X position, man kan inte röra sig upp eller ner så man behöver bara använda x positionen. Y är för skott etc
    };
    class asteroids
    {
        public int x, y;        // generationsplats
        public int speed;
    }

    class bullet
    {
        public bool active = false;
        public int time = 0;
        public int x, y;
    }

    class Program
    {
        // Importerar funktionen i DLL user32.dll, innehavar nödvändiga komponenter för programmet.
        [System.Runtime.InteropServices.DllImport("user32.dll")]

        // Säger att vi har den här funktionen.
        static extern short GetAsyncKeyState(int vKey);

        // Definerar tangenter för att göra det lättare att läsa, icke nödvändigt.
        static int VK_RIGHT = 0x27; // Right Key
        static int VK_LEFT = 0x25; // Left Key
        static int VK_UP = 0x26;   // Up Key

        static players player = new players();

        static bullet[] bullets = new bullet[16];

        static bool update = true;


        static void print()
        {
            if (!update)
                return;
            string buf = ""; // text buffer for smoother drawing
            for (int y = 0; y < 15; y++) // Y
            {
                for (int x = 0; x < 10; x++)    // X
                {
                    //for (int i = 0; i < asteroids..num; i++) { }
                    bool drawn = false;

                    if (y == player.y && x == player.x)
                    {
                        buf += "  O  ";
                        drawn = true;
                    }

                    if (!drawn)
                    {
                        for (int i = 0; i < bullets.Length; i++)
                        {
                            if (!bullets[i]?.active ?? 1)
                                continue;
                            buf += "  I  ";
                        }
                    }
                    if (!drawn)
                        buf += "     ";
                }
                buf += "  | \n";
            }
            Console.Clear();
            Console.WriteLine(buf);
            update = false;
        }

        static void keyPresses()
        {
            byte[] result_right = BitConverter.GetBytes(GetAsyncKeyState(VK_RIGHT)); // Nödvändigt enligt då när man trycker en tangent representeras det som 0000, beroende på vad du trycker kan det vara, 0010, 1011, 1001.
            byte[] result_up = BitConverter.GetBytes(GetAsyncKeyState(VK_UP));
            byte[] result_left = BitConverter.GetBytes(GetAsyncKeyState(VK_LEFT)); // Enter



            if (result_up[0] == 1) //Kollar om knappen tryckts ner sedan den kollade sist
            {
                update = true;

            }

            if (result_right[0] == 1)
            {
                player.x++;
                update = true;
            }

            if (result_left[0] == 1)
            {
                player.x--;
                update = true;
            }

            if (player.x > 9)
                player.x = 9;
            if (player.x < 0)
                player.x = 0;

        }

        static void Main(string[] args)
        {
            while (true)
            {
                keyPresses();
                print();
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}

