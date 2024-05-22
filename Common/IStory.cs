using DataStore;
using StandardLibrary;

namespace Common
{
    public interface IStory
    {
        World World { get; }
        Core Core { get; }
        Player Player { get; }
        void InitializeWorld();
    }
}
