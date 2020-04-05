<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PackageSelectionControl.ascx.cs" Inherits="WebControls_PackageSelectionControl" %>

 <div style="width: 300px;">
    <div class="">
         <div style="margin-left:5px; padding-top:4px;">
              <img width="300px" height="35px" src="/Images/get_hangout_cash.png" />
              <%--<asp:Image id="imgType" runat="server"  /> --%>
         </div>
         <div> 
            <asp:Literal runat="server" id="packageMsg1" /> 
            <asp:Literal runat="server" id="packageMsg2" /> 
        </div>
    </div>
    <div class="cashAmountSelector">   
        <asp:Literal runat="server" ID="headerMsg" Visible ="false" />
        <asp:Repeater id="rptPackage" runat="server" OnItemDataBound="rptPackage_ItemDataBound" >
        <ItemTemplate> 
            <hr style="height:1px;border-width:0;color:#E6E190;background-color:#E6E190"  />
            <asp:HiddenField ID="hIndex" Value='<%# DataBinder.Eval(Container.DataItem, "index") %>' Runat="server" />
            <asp:HiddenField ID="hSelect" Value='<%# DataBinder.Eval(Container.DataItem, "id") %>' Runat="server" />
            <asp:RadioButton CssClass="largerRadioButton" ID="rbSelect" Runat="server" GroupName="groupPackage" ></asp:RadioButton> 
            <span style="margin-left:5px;"><asp:label Width="50px" runat="server"  Text='<%# DataBinder.Eval(Container.DataItem, "vMoney") %>'></asp:label></span>
            <asp:Image Width="20px" Height="15px" id="imgTypeSelect" ImageUrl='<%# DataBinder.Eval(Container.DataItem, "imageUrl") %>' AlternateText='<%# DataBinder.Eval(Container.DataItem, "imageAltText") %>' runat="server"  />    
            <span style="margin-left:30px"><asp:Literal Runat="server" ID="spacer" Text=" " /></span>
            $<%# DataBinder.Eval(Container.DataItem, "usd") %> <asp:image runat="server" alt="bestValue" id="imgBestValue" visible='<%# ShowBestValueImage(DataBinder.Eval(Container.DataItem, "bestValue").ToString())%>' style="vertical-align:text-bottom; margin-left:10px;" width="89px" height="23px" src="/Images/best_value.png" />
         </ItemTemplate> 
        </asp:Repeater> 
   </div>
</div>