using System;

namespace LD34 {
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program {
        private static LD34Game game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            using (game = new LD34Game())
                game.Run();
        }

        public static void Restart() {
            game.Exit();
            using (game = new LD34Game())
                game.Run();
        }
    }
#endif
}
