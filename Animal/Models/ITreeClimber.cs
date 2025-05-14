namespace Laba._2_Animal.Models

{
    public interface ITreeClimber
    {
        bool IsOnTree { get; }

        void ClimbTree();

        void GetDownFromTree();
    }
}