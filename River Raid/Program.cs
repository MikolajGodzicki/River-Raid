using System;

namespace River_Ride___MG {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new Main())
                game.Run();

        }
    }
}
