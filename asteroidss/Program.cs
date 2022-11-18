using System;

namespace asteroidss
{

    class players
    {
        public int x = 5, y = 14; // X position, man kan inte röra sig upp eller ner så man behöver bara använda x positionen. Y är för skott etc
    };
    class asteroids
    {
        public int x, y;        // generationsplats
        public bool bActive = false;
        public int speed;
    }

    class bullet
    {
        public bool bActive = false;
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

        static asteroids[] asteroid = new asteroids[16];

        static string path = System.AppDomain.CurrentDomain.BaseDirectory;

        static bool update = true;

        static int score = 0;
        static int lives = 3;

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
                        for (int i = 0; i < asteroid.Length; i++)
                        {
                            if (!asteroid[i].bActive) // Check if bullet has been shot
                                continue;
                            if (x == asteroid[i].x && y == asteroid[i].y)
                            {
                                buf += "  M  ";
                                drawn = true;
                            }
                        }
                    }

                    if (!drawn)
                    {
                        for (int i = 0; i < bullets.Length; i++)
                        {

                            if (!bullets[i].bActive) // Check if bullet has been shot
                                continue;
                            if(x == bullets[i].x && y == bullets[i].y)
                            {

                                buf += "  I  ";
                                drawn = true;
                            }
                        }
                    }
                    if (!drawn)
                        buf += "     ";

                }

                switch (y)
                {
                    case 3:
                        buf += "  |      SCORE : " + score + "\n";
                        break;
                    case 4:
                        buf += "  |      LIVES : " + lives + "\n";
                        break;
                    default:
                        buf += "  | \n";
                        break;
                }
            }
            Console.Clear();
            Console.WriteLine(buf);
            update = false;
        }

        static void asteroid_gen()
        {
            Random rnd = new Random();
            int placeholder = rnd.Next(0, 10);

            if (!(placeholder <= 3)) // 30% chance of asteroid spawning
                return;
            for(int i = 0; i < asteroid.Length; i++)
            {
                if (asteroid[i].bActive)
                    continue;
                placeholder = rnd.Next(0, 10);
                asteroid[i].x = placeholder;
                asteroid[i].y = 0;
                asteroid[i].bActive = true;
                update = true;
                break;
            }
            return;
        }

        static void keyPresses()
        {
            byte[] result_right = BitConverter.GetBytes(GetAsyncKeyState(VK_RIGHT)); // Nödvändigt enligt då när man trycker en tangent representeras det som 0000, beroende på vad du trycker kan det vara, 0010, 1011, 1001.
            byte[] result_up = BitConverter.GetBytes(GetAsyncKeyState(VK_UP));
            byte[] result_left = BitConverter.GetBytes(GetAsyncKeyState(VK_LEFT)); // Enter

            if (result_up[0] == 1) //Kollar om knappen tryckts ner sedan den kollade sist
            {
                for(int i = 0; i < bullets.Length; i++)
                {
                    if (bullets[i].bActive)
                        continue;
                    bullets[i].bActive = true;
                    bullets[i].x = player.x;
                    bullets[i].y = player.y - 1;
                    break;
                }
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

        static int Main(string[] args) // Prog start
        {
            for (int i = 0; i < bullets.Length; i++) // initializing all the objects with the data we want.
            {
                bullets[i] = new bullet();
                asteroid[i] = new asteroids();
            }

            int tick = 0; // Game unit of time
            while (true)
            {
                keyPresses();
                if(tick % 4 == 0) asteroid_gen(); // Every 4 ticks create an enemy
                print();

                // Check if you have lost after the print updates, that way the shown stats are all up to date etc.
                if (lives <= 0)
                {
                    Console.WriteLine("You Lost!, Your highscore was " + score + "!");
                    System.Threading.Thread.Sleep(3000);
                    return 0;
                } 
                System.Threading.Thread.Sleep(20);
                tick++;
                for(int i = 0; i < bullets.Length; i++)
                {
                    for (int op = 0; op < asteroid.Length; op++)
                    {
                        if (!bullets[i].bActive || !asteroid[op].bActive) continue;
                        if (bullets[i].x == asteroid[op].x && bullets[i].y <= asteroid[op].y)
                        {
                            bullets[i].bActive = false;
                            asteroid[op].bActive = false;
                            score++;
                        }
                    }
                    

                    if (bullets[i].bActive) bullets[i].y -= 1;
                    if (asteroid[i].bActive && (tick % 4 == 0)) asteroid[i].y += 1;
                    update = true;
                    if (bullets[i].y < 0)
                        bullets[i].bActive = false;
                    if (asteroid[i].y > 16 && asteroid[i].bActive)
                    {
                        asteroid[i].bActive = false;
                        lives--;
                    }       
                }

                // Current high score = 253;

            }
        }
    }
}


