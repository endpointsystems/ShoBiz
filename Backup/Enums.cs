// Copyright (c) 2007-2009 Endpoint Systems. All rights reserved.
// 
// THE PROGRAM IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT WITHOUT ANY WARRANTY. IT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
// 
// IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW THE AUTHOR WILL BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS), EVEN IF THE AUTHOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
// 
// 
namespace EndpointSystems.OrchestrationLibrary
{
    /// <summary>
    /// Enumerates the type of link the shape has to its parent, if provided within the orchestration shape.
    /// </summary>
    public enum ParentLink
    {
        /// <summary>
        /// Module_*
        /// </summary>
        Module,
        /// <summary>
        /// ServiceDeclaration_* and ServiceBody_*
        /// </summary>
        Service,
        /// <summary>
        /// Scope_*
        /// </summary>
        Scope,
        /// <summary>
        /// Port* and Port_*
        /// </summary>
        Port,
        /// <summary>
        /// CorrelationDeclaration_StatementRef
        /// </summary>
        Correlation,
        /// <summary>
        /// CorrelationType_PropertyRef
        /// </summary>
        CorrelationProperty,
        /// <summary>
        /// Construct_MessageRef
        /// </summary>
        ConstructMessageRef,
        /// <summary>
        /// MultipartMessageType_*
        /// </summary>
        MultiPartMessage,
        /// <summary>
        /// Operation_*
        /// </summary>
        Operation,
        /// <summary>
        /// ComplexStatement_Statement, others?
        /// </summary>
        Statement,
        /// <summary>
        /// DNFPredicate
        /// </summary>
        FilterPredicate,
        /// <summary>
        /// Transform_InputMessagePartRef
        /// </summary>
        InputMessagePart,
        /// <summary>
        /// Transform_OutputMessagePartRef
        /// </summary>
        OutputMessagePart,
        /// <summary>
        /// CallRules_RulesParameterRef
        /// </summary>
        RulesParameterRef,
        /// <summary>
        /// For those we do not know of...
        /// </summary>
        Unknown,
        /// <summary>
        /// For those with no parent link attributes
        /// </summary>
        None
    }

    //////////////////////////////////////////////////
    ///Module_ServiceDeclaration
    ///ServiceDeclaration_ServiceBody
    /// CorrelationDeclaration_StatementRef
    ///ServiceBody_Statement
    ///Scope_VariableDeclaration
    ///ServiceDeclaration_MessageDeclaration
    ///ServiceDeclaration_VariableDeclaration
    ///ServiceDeclaration_PortDeclaration
    ///PortDeclaration_CLRAttribute
    ///ServiceDeclaration_PortDeclaration
    ///MultipartMessageType_PartDeclaration
    ///OperationDeclaration_RequestMessageRef
    ///CorrelationType_PropertyRef
    ///Module_CorrelationType
    ///ServiceDeclaration_ServiceLinkDeclaration - 
    /// <summary>
    /// Enumerate the different Message object directions found in orchestrations.
    /// </summary>
    public enum MessageDirection
    {
        In,
        Out,
        InOut,
        Indeterminant //error condition
    }

    /// <summary>
    /// Used by BtsOperationDeclaration object to determine operation type.
    /// See Microsoft.BizTalk.ExplorerOM.OperationType for 'original'
    /// </summary>
    public enum OperationType
    {
        None,
        Notification,
        OneWay,
        RequestResponse,
        SolicitResponse
    }

    /// <summary>
    /// used by the child objects of BindingAttribute to determine binding type
    /// </summary>
    public enum BindingAttributeType
    {
        Logical,
        Direct,
        Physical
    }

    public enum IsolationType
    {
        Serializable,
        ReadRepeatable,
        ReadCommitted,
    }
}