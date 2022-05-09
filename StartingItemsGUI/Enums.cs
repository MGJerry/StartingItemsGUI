namespace StartingItemsGUI
{
    namespace Enums
    {
        public enum EarningMode : byte
        {
            Stages = 0,
            Bosses,
            GameEnding
        }

        public enum ShopMode : byte
        {
            EarnedConsumable = 0,
            EarnedPersistent,
            Free,
            Random,
            Uninitialized = byte.MaxValue
        }
    }
}
