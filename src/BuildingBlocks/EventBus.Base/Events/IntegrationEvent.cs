using System.Text.Json.Serialization;

namespace EventBus.Base.Events
{
    public class IntegrationEvent
    {
        #region Public Properties

        public Guid Id { get; private set; }

        public DateTime CreatedDate { get; private set; }

        #endregion

        #region Constructors and Destructors

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime dateTime)
        {
            Id = id;
            CreatedDate = dateTime;
        }

        #endregion
    }
}
