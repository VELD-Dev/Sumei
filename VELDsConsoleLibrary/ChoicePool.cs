namespace ConsoleLib;

#pragma warning disable CS8602
public class ChoicePool
{
    public readonly string Id;

    /// <summary>
    /// Can be left empty if the desired text is already logged before in the console.
    /// </summary>
    public string Text;

    /// <summary>
    /// If true, multiple choices can be selected.
    /// </summary>
    public bool Multichoice;

    /// <summary>
    /// Index of the currently active choice.
    /// </summary>
    public int activeChoice = -1;

    public List<Choice> selectedChoices = new();

    /// <summary>
    /// Dictionary of choices the pool contains and shows on screen.
    /// </summary>
    public Dictionary<string, Choice> Choices = new();

    /// <summary>
    /// True once the pool has ended.
    /// </summary>
    public bool Ended { get; private set; } = false;

    /// <summary>
    /// Gives the list of selected choices. If the pool was not multichoice, the array will only contain the selected choice.
    /// </summary>
    public event Action<Choice[]>? OnPoolEnd;

    /// <summary>
    /// Event that occurs when the selection changes. On single-choice pools, you better use <see cref="OnPoolEnd"/>.
    /// </summary>
    public event Action<Choice, bool>? OnSelectionChange;

    public event Action<Choice>? OnHoverChoice;

    public ChoicePool(string id, string text, bool multichoice = false)
    {
        Id = id;
        Text = text;
        Multichoice = multichoice;
    }

    public ChoicePool(string id, string text, bool multichoice, Dictionary<string, Choice> choices) : this(id, text, multichoice)
    {
        Choices = choices;
    }

    public ChoicePool AddChoice(string id, string text)
    {
        var c = new Choice(id, text, this);
        Choices.Add(id, c);
        return this;
    }

    public ChoicePool AddChoice(string id, string text, Func<bool> enabled)
    {
        var c = new Choice(id, text, this, enabled);
        Choices.Add(id, c);
        return this;
    }

    public ChoicePool AddChoice(Choice choice)
    {
        Choices.Add(choice.Id, choice);
        return this;
    }

    public void EndPool()
    {
        Ended = true;
        OnPoolEnd(selectedChoices.ToArray());
    }

    public Choice? GetChoiceFromIndex(int index)
    {
        try
        {
            var res = Choices.ElementAt(index);
            return res.Value;
        }
        catch(Exception e)
        {
            AnsiConsole.Markup($"[red]{e}[/]");
            return null;
        }
    }

    public void KeyDown()
    {
        if (activeChoice + 1 > Choices.Count - 1)
            return;

        activeChoice++;

        var hoveredChoice = GetChoiceFromIndex(activeChoice);

        if (hoveredChoice == null)
            return;

        OnHoverChoice(hoveredChoice.Value);
    }

    public void KeyUp()
    {
        if (activeChoice - 1 < Choices.Count - 1)
            return;

        activeChoice--;

        var hoveredChoice = GetChoiceFromIndex(activeChoice);

        if (hoveredChoice == null)
            return;

        OnHoverChoice(hoveredChoice.Value);
    }

    public void KeyEnter()
    {
        if (activeChoice == -1) return;
        if (activeChoice > Choices.Count - 1) return;

        var hoveredChoice = GetChoiceFromIndex(activeChoice);
        if (hoveredChoice == null) return;

        if (Multichoice)
        {
            hoveredChoice.Value.Select();
            if (hoveredChoice.Value.selectionState && selectedChoices.Contains(hoveredChoice.Value))
            {
                selectedChoices.Remove(hoveredChoice.Value);
            }
            else if(hoveredChoice.Value.selectionState == false && !selectedChoices.Contains(hoveredChoice.Value))
            {
                selectedChoices.Add(hoveredChoice.Value);
            }
            else
            {
                Console.WriteLine("[ERROR] For some reason, the hovered choice was selected and the selectedChoices did not contain it, or vice-versa. Aborting.");
                return;
            }
        }
    }
}