using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleLib;

public struct Choice
{
    /// <summary>
    /// Identifier of the choice. Changing this field manually can break choice pools.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Text of the choice, basically the choice itself.
    /// </summary>
    public string Text;

    /// <summary>
    /// If this delegate returns false, the choice will be grayed out and unselectable.
    /// </summary>
    public Func<bool> Enabled = () => true;

    /// <summary>
    /// <see cref="ChoicePool"/> owning this choice.
    /// </summary>
    public ChoicePool Pool { get; private set; }

    public bool selectionState = false;

    /// <summary>
    /// The <see cref="bool"/> defines if the choice is selected or deselected. For single choice pools, this is true by default.
    /// </summary>
    public event Action<bool> OnSelectionChange;

    /// <summary>
    /// Creates a choice.
    /// </summary>
    /// <param name="id">Identifier of the choice.</param>
    /// <param name="text">Text of the choice</param>
    /// <param name="owningPool"><see cref="ChoicePool"/> owning this choice.</param>
    [SetsRequiredMembers]
    public Choice(string id, string text, ChoicePool owningPool)
    {
        Id = id;
        Text = text;
        Pool = owningPool;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="Choice(string, string, ChoicePool)" path="/param[@name='id']"/></param>
    /// <param name="text"><inheritdoc cref="Choice(string, string, ChoicePool)" path="/param[@name='text']"/></param>
    /// <param name="owningPool"><inheritdoc cref="Choice(string, string, ChoicePool)" path="/param[@name='owningPool']"/></param>
    /// <param name="enabled">Delegate that defines if the choice should be selectable or not (and then grayed out)</param>
    [SetsRequiredMembers]
    public Choice(string id, string text, ChoicePool owningPool, Func<bool> enabled) : this(id, text, owningPool)
    {
        Enabled = enabled;
    }

    public void Select()
    {
        selectionState = !selectionState;
        OnSelectionChange(selectionState);
    }
}
