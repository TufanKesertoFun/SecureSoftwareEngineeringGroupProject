using System;
using System.IO;
using System.Text;
using SharpFuzz;

public static class Program
{
    public static void Main()
    {
        // Visible banner so we know the fuzz harness started
        Console.Error.WriteLine("== SharpFuzz starting (friendly banner) ==");
        Console.Error.WriteLine("Using corpus folder: .\\corpus");
        Console.Error.Flush();

        Fuzzer.Run((Stream s) =>
        {
            // read test case bytes
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            byte[] data = ms.ToArray();
            if (data.Length == 0) return;

            // map -> username / password
            string all = Encoding.Latin1.GetString(data);
            string username = "user", password = "pass";
            if (all.Contains("username:") && all.Contains("password:"))
            {
                var lines = all.Replace("\r", "").Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("username:")) username = line["username:".Length..].Trim();
                    if (line.StartsWith("password:")) password = line["password:".Length..].Trim();
                }
            }
            else
            {
                int split = Math.Max(1, data.Length / 2);
                username = Encoding.Latin1.GetString(data, 0, split);
                password = Encoding.Latin1.GetString(data, split, data.Length - split);
            }

            // *** Visible per-run log (prints to stderr) - safe for short runs ***
            Console.Error.WriteLine($"[FUZZ] username='{Truncate(username, 40)}' password_len={password.Length}");
            Console.Error.Flush();

            // call code under test
            bool ok = AuthService.Authenticate(username, password);
            if (ok) ProtectedResource.Access(username);
        });
    }

    // helper to keep console lines short
    static string Truncate(string s, int n) => s == null ? "(null)" : (s.Length <= n ? s : s.Substring(0, n) + "…");
}

// placeholders - keep as-is for fuzzing
public static class AuthService
{
    public static bool Authenticate(string username, string password)
    {
        if (username == null || password == null) throw new ArgumentNullException();
        if (username.Length > 256) throw new InvalidOperationException("Username too long.");
        if (password.Contains("\0")) throw new Exception("NUL in password");
        return username == "admin" && password == "admin";
    }
}
public static class ProtectedResource
{
    public static void Access(string user)
    {
        if (user.StartsWith("A") && user.EndsWith("!")) throw new Exception("Edge after auth");
    }
}
