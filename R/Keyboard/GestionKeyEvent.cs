using System;
using System.Threading;
using System.Windows.Input;

namespace R
{
    public class GestionKeyEvent
    {
        public void GetPressKey(Action action)
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    action();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public bool KeyDown(string touche = null)
        {
            try
            {
                if (touche.IsNullOrEmpty()) return false;
                touche = touche.ToLower();

                switch (touche)
                {
                    case "a":
                        return Keyboard.IsKeyDown(Key.A);
                    case "b":
                        return Keyboard.IsKeyDown(Key.B);
                    case "c":
                        return Keyboard.IsKeyDown(Key.C);
                    case "d":
                        return Keyboard.IsKeyDown(Key.D);
                    case "e":
                        return Keyboard.IsKeyDown(Key.E);
                    case "f":
                        return Keyboard.IsKeyDown(Key.F);
                    case "g":
                        return Keyboard.IsKeyDown(Key.G);
                    case "h":
                        return Keyboard.IsKeyDown(Key.H);
                    case "i":
                        return Keyboard.IsKeyDown(Key.I);
                    case "j":
                        return Keyboard.IsKeyDown(Key.J);
                    case "k":
                        return Keyboard.IsKeyDown(Key.K);
                    case "l":
                        return Keyboard.IsKeyDown(Key.L);
                    case "m":
                        return Keyboard.IsKeyDown(Key.M);
                    case "n":
                        return Keyboard.IsKeyDown(Key.N);
                    case "o":
                        return Keyboard.IsKeyDown(Key.O);
                    case "p":
                        return Keyboard.IsKeyDown(Key.P);
                    case "q":
                        return Keyboard.IsKeyDown(Key.Q);
                    case "r":
                        return Keyboard.IsKeyDown(Key.R);
                    case "s":
                        return Keyboard.IsKeyDown(Key.S);
                    case "t":
                        return Keyboard.IsKeyDown(Key.T);
                    case "u":
                        return Keyboard.IsKeyDown(Key.U);
                    case "v":
                        return Keyboard.IsKeyDown(Key.V);
                    case "w":
                        return Keyboard.IsKeyDown(Key.W);
                    case "x":
                        return Keyboard.IsKeyDown(Key.X);
                    case "y":
                        return Keyboard.IsKeyDown(Key.Y);
                    case "z":
                        return Keyboard.IsKeyDown(Key.Z);
                    default:
                        return false;
                }
                
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
