namespace Project.Compiler
{
    public enum BasicCommand {
        LET,
        DIM,
        READ,
        ARITHMETIC,
        

    }


    public class ArmCommand
    {
        public BasicCommand Origin { get; set; }

    }
}