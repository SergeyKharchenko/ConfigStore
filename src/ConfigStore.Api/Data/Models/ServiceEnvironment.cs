namespace ConfigStore.Api.Data.Models {
    public class ServiceEnvironment : ModelBase {
        public virtual ApplicationService Service { get; set; }

        public int ServiceId { get; set; }
    }
}