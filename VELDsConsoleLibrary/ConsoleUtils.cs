namespace ConsoleLib;

public static class ConsoleUtils
{
    public static void WaitForKey(ConsoleKey key, bool intercept = false)
    {
        var continueproc = false;
        while(continueproc == false)
        {
            var typedKey = Console.ReadKey(intercept);
            if (typedKey.Key != key)
                continue;
            continueproc = true;
        }
    }

    public static void ExecuteWhenKeyPressed(ConsoleKey key, Action<ConsoleKeyInfo> action, bool intercept = false)
    {
        var execute = false;
        ConsoleKeyInfo pressedKey = new();
        while(execute == false)
        {
            pressedKey = Console.ReadKey(intercept);
            if (pressedKey.Key != key)
                continue;
            execute = true;
        }
        action.Invoke(pressedKey);
    }

    public static void ExecuteWhenKeyPressed((ConsoleKey, Action<ConsoleKeyInfo>)[] values, bool intercept = false)
    {
        var continueProc = false;
        ConsoleKeyInfo pressedKey = new();
        while(continueProc == false)
        {
            pressedKey = Console.ReadKey(intercept);
            foreach(var val in values)
            {
                if (pressedKey.Key != val.Item1)
                    continue;

                val.Item2.Invoke(pressedKey);
                continueProc = true;
            }
        }
    }
}
