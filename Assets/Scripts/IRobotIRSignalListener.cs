public interface IRobotIRSignalListener
{
    void OnRobotNearby(Robot robot, IrDirection drn, bool isReceiving, bool force);
}