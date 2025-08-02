namespace Acme.Packages.Menu
{
    public interface ITextEditor
	{
		string Content
		{
			get;
			set;
		}

		bool IsDirty
		{
			get;
			set;
		}
	}
}
