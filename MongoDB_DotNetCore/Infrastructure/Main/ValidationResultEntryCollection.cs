using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

public class ValidationResultEntryCollection
{
    public ValidationResultEntryCollection() {}

    public void AddToList(string validationTarget, string message)
    {
        ValidationResultEntry entry = new ValidationResultEntry
        {
            ValidationMessage = message,
            ValidationTarget = validationTarget
        };

        AddToList(entry);
    }

    public void AddToList(ValidationResultEntry entry)
    {
        m_lstValidationResultEntries.Add(entry);
    }

    public string FormulateMessageFromList()
    {
        StringBuilder sb = new StringBuilder();

        foreach(ValidationResultEntry entry in m_lstValidationResultEntries)
        {
            if (sb.Length > 0) sb.Append("<br><br>");
            sb.Append(entry.ValidationMessage);
        }

        return sb.ToString();
    }

    public void AddOtherCollection(ValidationResultEntryCollection otherCollection)
    {
        m_lstValidationResultEntries.AddRange(otherCollection.m_lstValidationResultEntries);
    }

    public int Count => m_lstValidationResultEntries.Count;

    public ValidationResultEntry At(int index)
    {
        return m_lstValidationResultEntries[index];
    }

    private readonly List<ValidationResultEntry> m_lstValidationResultEntries = new List<ValidationResultEntry>();
}
