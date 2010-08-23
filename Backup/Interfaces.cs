// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 

#region

using System.Collections.Generic;

#endregion

namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// grandfather interface
    /// </summary>
    public interface IBtsBaseComponent
    {
    }

    /// <summary>
    /// BizTalk orchestration interface
    /// </summary>
    public interface IBtsOrch : IBtsBaseComponent
    {
    }

    /// <summary>
    /// interface for the Application logical container
    /// </summary>
    public interface IBtsApp : IBtsBaseComponent
    {
    }

    /// <summary>
    /// orchestration variables for orchestration instances
    /// </summary>
    public interface IBtsOrchVariable : IBtsBaseComponent
    {
    }

    /// <summary>
    /// instance message variables for orchestration instances
    /// </summary>
    public interface IBtsOrchMsg : IBtsBaseComponent
    {
    }

    /// <summary>
    /// Orchestration shapes that can contain other orchestration shapes
    /// </summary>
    public interface IBtsContainer : IBtsShape
    {
        List<BtsBaseComponent> Components { get; }
    }

    /// <summary>
    /// Your average, everyday, run-of-the-mill orchestration shape
    /// </summary>
    public interface IBtsShape : IBtsBaseComponent
    {
    }

    public interface IBtsDecisionBranch : IBtsShape
    {
    }

    /// <summary>
    /// PortType interface
    /// </summary>
    public interface IBtsPortType : IBtsBaseComponent
    {
    }

    public interface IBtsOperationDeclaration : IBtsBaseComponent
    {
    }

    public interface IBtsMessageRef : IBtsBaseComponent
    {
    }

    public interface IBtsMultiPartMessageType : IBtsBaseComponent
    {
    }

    public interface IBtsPartDeclaration : IBtsBaseComponent
    {
    }

    public interface IBtsCorrelationDeclaration : IBtsBaseComponent
    {
    }

    public interface IBtsCorrelationType : IBtsBaseComponent
    {
    }

    public interface IBtsStatementRef : IBtsBaseComponent
    {
    }

    public interface IBtsPropertyRef : IBtsBaseComponent
    {
    }

    public interface IBtsMessageDeclaration : IBtsBaseComponent
    {
    }

    public interface IBtsPortDeclaration : IBtsBaseComponent
    {
    }

    public interface IBtsMessagePartRef : IBtsBaseComponent
    {
    }

    public interface IBtsFilter
    {
    }
}