using System.Collections.Generic;

public class MenuOption
{
    string OptionText { get; set; } = string.Empty;
    List<MenuOption> ChildOptions { get; set; } = new List<MenuOption>();
}
