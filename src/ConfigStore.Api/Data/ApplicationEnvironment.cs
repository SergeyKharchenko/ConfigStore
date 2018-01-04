namespace ConfigStore.Api.Data {
    public class ApplicationEnvironment {
        public string Name { get; set; }

        public virtual Application Application { get; set; }

        public int ApplicationId { get; set; }
    }
}