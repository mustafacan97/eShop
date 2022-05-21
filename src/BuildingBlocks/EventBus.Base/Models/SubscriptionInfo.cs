namespace EventBus.Base.Models
{
    public class SubscriptionInfo
    {
        #region Public Properties
        
        public Type HandlerType { get; }

        #endregion

        #region Constructors and Destructors

        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }

        #endregion

        #region Public Methods

        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }

        #endregion
    }
}
