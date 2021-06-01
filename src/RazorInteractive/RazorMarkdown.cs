namespace RazorInteractive
{
    public class RazorMarkdown
    {
        public RazorMarkdown(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => Value;
    }
}