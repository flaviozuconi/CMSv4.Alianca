namespace VM2.PageSpeed
{
    public class BLPageSpeedUtil : BLPageSpeedUtilBase
    {
    }

    public class BLPageSpeedAuditUtil : BLPageSpeedUtilBase
    {
        public override string ColorGreen => "fa fa-circle text-green";

        public override string ColorYellow => "fa fa-square text-orange";

        public override string ColorRed => "fa fa-caret-up text-red";
    }
}
