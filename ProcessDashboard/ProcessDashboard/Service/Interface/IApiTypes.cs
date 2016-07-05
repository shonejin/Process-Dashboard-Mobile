namespace ProcessDashboard.Service.Interface
{
    /*
     DO NOT CHANGE

        This interface has the list of API Types that is provided by Fusilade.
        Each type has a priority associated with it. 
        *Priority ordering*
        UserInitiated > Background > Speculatinve

    */
	public interface IApiTypes
	{
        IPDashApi Speculative { get; }
        IPDashApi UserInitiated { get; }
        IPDashApi Background { get; }
	}
}