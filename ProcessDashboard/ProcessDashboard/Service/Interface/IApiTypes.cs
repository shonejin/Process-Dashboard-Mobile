#region
using Fusillade;
#endregion
namespace ProcessDashboard.Service.Interface
{
    /*
     * ** DO NOT CHANGE **
     * 
     *  Name: IApiTypes.cs
     *  
     *  Purpose: This interface is to make use of Fusilade. 
     *  
     *  Description:   
     *  
     *   This interface has the list of API Types that is provided by Fusilade.
     *   Each type has a priority associated with it. 
     *   ** Priority ordering **
     *   UserInitiated > Background > Speculative
    */
    public interface IApiTypes
    {
        IPDashApi Speculative { get; }
        IPDashApi UserInitiated { get; }
        IPDashApi Background { get; }
        IPDashApi GetApi(Priority priority);
    }
}