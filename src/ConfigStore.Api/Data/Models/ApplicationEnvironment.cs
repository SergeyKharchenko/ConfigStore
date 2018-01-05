namespace ConfigStore.Api.Data.Models {
    public class ApplicationEnvironment {
        public string Name { get; set; }

        public virtual Application Application { get; set; }

        public int ApplicationId { get; set; }
    }
}