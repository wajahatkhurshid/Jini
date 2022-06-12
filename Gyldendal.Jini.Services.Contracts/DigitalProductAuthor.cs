namespace Gyldendal.Jini.Services.Contracts
{
    public class DigitalProductAuthor
    {
        /// <summary>
        /// Product Author Full Name
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string FullName => AuthorName;

        public string AuthorName { get; set; }
    }
}
