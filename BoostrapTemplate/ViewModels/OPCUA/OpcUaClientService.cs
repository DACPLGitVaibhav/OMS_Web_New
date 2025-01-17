using System;
using System.Linq;
using System.Threading.Tasks;
using DATA.Interaces;
using OMS_Template.ViewModels.OPCUA;
using OMS_Web.Controllers.DataVisulization;
using Opc.Ua;
using Opc.Ua.Client;

public class OpcUaClientService
{
    public Session _session;
    public async Task<bool> ConnectAsync()
    {
        try
        {
            Utils.SetTraceOutput(Utils.TraceOutput.Off);
            var config = new ApplicationConfiguration()
            {
                ServerConfiguration = new ServerConfiguration
                {
                    UserTokenPolicies = new UserTokenPolicyCollection(new[] { new UserTokenPolicy(UserTokenType.UserName) }),
                },
                ApplicationName = "MyConfig",
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = @"Windows",
                        StorePath = @"CurrentUser\My",
                        SubjectName = Utils.Format(@"CN={0}, DC={1}", "MyHomework", System.Net.Dns.GetHostName())
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = @"Windows",
                        StorePath = @"CurrentUser\TrustedPeople",
                    },
                    NonceLength = 32,
                    AutoAcceptUntrustedCertificates = true
                },
                //TransportConfigurations = new TransportConfigurationCollection(),
                //TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { }
            };

            //_ = config.Validate(ApplicationType.Client);
            //if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            //{
            //    config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = true; };
            //}

            config.CertificateValidator = new CertificateValidator();
            config.CertificateValidator.CertificateValidation += (s, certificateValidationEventArgs) =>
            {
                certificateValidationEventArgs.Accept = true; // Accept all certificates for testing purposes; modify this for production.
            };

            // Create a new session with the OPC UA server asynchronously
            _session = await Session.Create(config, new ConfiguredEndpoint(null, new EndpointDescription("opc.tcp://192.168.196.1:4840")), true, "", 15000, new UserIdentity(), null);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<OPCUADetails> ReadNode()
    {
        OPCUADetails _oPCUADetails = new OPCUADetails();

        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    _oPCUADetails.Isconnect = true;
                    var dataFF = DataVisulizationController.FF.ToList();
                    var dataFE = DataVisulizationController.FE.ToList();
                    var dataRF = DataVisulizationController.RF.ToList();
                    var dataBSRH = DataVisulizationController.BSRH.ToList();
                    var dataBSLH = DataVisulizationController.BSLH.ToList();

                    NodeId node1 = new NodeId(dataFF[0].LOTSequence.ToString());
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _oPCUADetails.MPLC_FF = value1.Value.ToString();

                    NodeId node2 = new NodeId(dataFE[0].LOTSequence.ToString());
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _oPCUADetails.MPLC_FE = value2.Value.ToString();

                    NodeId node3 = new NodeId(dataRF[0].LOTSequence.ToString());
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _oPCUADetails.MPLC_RF = value3.Value.ToString();

                    NodeId node4 = new NodeId(dataBSRH[0].LOTSequence.ToString());
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _oPCUADetails.MPLC_BSRH = value4.Value.ToString();

                    NodeId node5 = new NodeId(dataBSLH[0].LOTSequence.ToString());
                    DataValue value5 = await _session.ReadValueAsync(node5);
                    _oPCUADetails.MPLC_BSLH = value5.Value.ToString();
                }
                catch (Exception)
                {

                    _oPCUADetails = new OPCUADetails();
                    _oPCUADetails.Isconnect = false;
                    _oPCUADetails.MPLC_FF = "0";
                    _oPCUADetails.MPLC_FE = "0";
                    _oPCUADetails.MPLC_RF = "0";
                    _oPCUADetails.MPLC_BSRH = "0";
                    _oPCUADetails.MPLC_BSLH = "0";

                    _oPCUADetails.OMS_FF = "0";
                    _oPCUADetails.OMS_FE = "0";
                    _oPCUADetails.OMS_RF = "0";
                    _oPCUADetails.OMS_BSRH = "0";
                    _oPCUADetails.OMS_BSLH = "0";

                    await ConnectAsync();
                }
            }
        }
        
        return _oPCUADetails;
    }

    public async Task<bool> TestOPCSession()
    {
        bool b = false;
        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    NodeId node1 = new NodeId("ns=4;i=313");
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    b = true;
                }
                catch (Exception)
                {
                    _session.Dispose();
                    b = await ConnectAsync();

                }
            }
            else
            {
                _session.Dispose();
                b = await ConnectAsync();
            }
        }
        return b;
    }

    public async Task<Line_Order_mgmt_Status_Details> ReadNodeLinemgmtFF()
    {
        
        Line_Order_mgmt_Status_Details _LineMgmtDetails = new Line_Order_mgmt_Status_Details();

        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    var data = DataVisulizationController.FF.ToList();

                    NodeId node1 = new NodeId(data[0].LOTSequence.ToString());
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _LineMgmtDetails.LOT_Sequence = Convert.ToInt32(value1.Value);

                    NodeId node2 = new NodeId(data[0].LOTMesVcode.ToString());
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _LineMgmtDetails.LOT_Mes_Vcode = Convert.ToInt32(value2.Value);

                    NodeId node3 = new NodeId(data[0].LOSSequence.ToString());
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _LineMgmtDetails.LOS_Sequence = Convert.ToInt32(value3.Value);

                    NodeId node4 = new NodeId(data[0].LOSMesVcode.ToString());
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _LineMgmtDetails.LOS_Mes_Vcode = Convert.ToInt32(value4.Value);

                    NodeId node5 = new NodeId(data[0].LOPSequence.ToString());
                    DataValue value5 = await _session.ReadValueAsync(node5);
                    _LineMgmtDetails.LOP_Sequence = Convert.ToInt32(value5.Value);

                    NodeId node6 = new NodeId(data[0].LOPMesVcode.ToString());
                    DataValue value6 = await _session.ReadValueAsync(node6);
                    _LineMgmtDetails.LOP_Mes_Vcode = Convert.ToInt32(value6.Value);

                    NodeId PRGnode1 = new NodeId(data[0].PRGSeq1.ToString());
                    DataValue PRGvalue1 = await _session.ReadValueAsync(PRGnode1);
                    _LineMgmtDetails.PRG_Sequence1 = Convert.ToInt32(PRGvalue1.Value);

                    NodeId PRGnode2 = new NodeId(data[0].PRGMes1.ToString());
                    DataValue PRGvalue2 = await _session.ReadValueAsync(PRGnode2);
                    _LineMgmtDetails.PRG_Mes_Vcode1 = Convert.ToInt32(PRGvalue2.Value);

                    NodeId PRGnode3 = new NodeId(data[0].PRGSeq2.ToString());
                    DataValue PRGvalue3 = await _session.ReadValueAsync(PRGnode3);
                    _LineMgmtDetails.PRG_Sequence2 = Convert.ToInt32(PRGvalue3.Value);

                    NodeId PRGnode4 = new NodeId(data[0].PRGMes2.ToString());
                    DataValue PRGvalue4 = await _session.ReadValueAsync(PRGnode4);
                    _LineMgmtDetails.PRG_Mes_Vcode2 = Convert.ToInt32(PRGvalue4.Value);

                    NodeId PRGnode5 = new NodeId(data[0].PRGSeq3.ToString());
                    DataValue PRGvalue5 = await _session.ReadValueAsync(PRGnode5);
                    _LineMgmtDetails.PRG_Sequence3 = Convert.ToInt32(PRGvalue5.Value);

                    NodeId PRGnode6 = new NodeId(data[0].PRGMes3.ToString());
                    DataValue PRGvalue6 = await _session.ReadValueAsync(PRGnode6);
                    _LineMgmtDetails.PRG_Mes_Vcode3 = Convert.ToInt32(PRGvalue6.Value);

                    NodeId PRGnode7 = new NodeId(data[0].PRGSeq4.ToString());
                    DataValue PRGvalue7 = await _session.ReadValueAsync(PRGnode7);
                    _LineMgmtDetails.PRG_Sequence4 = Convert.ToInt32(PRGvalue7.Value);

                    NodeId PRGnode8 = new NodeId(data[0].PRGMes4.ToString());
                    DataValue PRGvalue8 = await _session.ReadValueAsync(PRGnode8);
                    _LineMgmtDetails.PRG_Mes_Vcode4 = Convert.ToInt32(PRGvalue8.Value);

                    NodeId PRGnode9 = new NodeId(data[0].PRGSeq5.ToString());
                    DataValue PRGvalue9 = await _session.ReadValueAsync(PRGnode9);
                    _LineMgmtDetails.PRG_Sequence5 = Convert.ToInt32(PRGvalue9.Value);

                    NodeId PRGnode10 = new NodeId(data[0].PRGMes5.ToString());
                    DataValue PRGvalue10 = await _session.ReadValueAsync(PRGnode10);
                    _LineMgmtDetails.PRG_Mes_Vcode5 = Convert.ToInt32(PRGvalue10.Value);

                    NodeId LOIPnode1 = new NodeId(data[0].LOIPSeq1.ToString());
                    DataValue LOIPvalue1 = await _session.ReadValueAsync(LOIPnode1);
                    _LineMgmtDetails.LOIP_Sequence1 = Convert.ToInt32(LOIPvalue1.Value);

                    NodeId LOIPVnode1 = new NodeId(data[0].LOIPMes1.ToString());
                    DataValue LOIPVvalue1 = await _session.ReadValueAsync(LOIPVnode1);
                    _LineMgmtDetails.LOIP_Mes_Vcode1 = Convert.ToInt32(LOIPVvalue1.Value);

                    NodeId LOIPnode2 = new NodeId(data[0].LOIPSeq2.ToString());
                    DataValue LOIPvalue2 = await _session.ReadValueAsync(LOIPnode2);
                    _LineMgmtDetails.LOIP_Sequence2 = Convert.ToInt32(LOIPvalue2.Value);

                    NodeId LOIPVnode2 = new NodeId(data[0].LOIPMes2.ToString());
                    DataValue LOIPVvalue2 = await _session.ReadValueAsync(LOIPVnode2);
                    _LineMgmtDetails.LOIP_Mes_Vcode2 = Convert.ToInt32(LOIPVvalue2.Value);

                    NodeId LOIPnode3 = new NodeId(data[0].LOIPSeq3.ToString());
                    DataValue LOIPvalue3 = await _session.ReadValueAsync(LOIPnode3);
                    _LineMgmtDetails.LOIP_Sequence3 = Convert.ToInt32(LOIPvalue3.Value);

                    NodeId LOIPVnode3 = new NodeId(data[0].LOIPMes3.ToString());
                    DataValue LOIPVvalue3 = await _session.ReadValueAsync(LOIPVnode3);
                    _LineMgmtDetails.LOIP_Mes_Vcode3 = Convert.ToInt32(LOIPVvalue3.Value);

                    NodeId LOIPnode4 = new NodeId(data[0].LOIPSeq4.ToString());
                    DataValue LOIPvalue4 = await _session.ReadValueAsync(LOIPnode4);
                    _LineMgmtDetails.LOIP_Sequence4 = Convert.ToInt32(LOIPvalue4.Value);

                    NodeId LOIPVnode4 = new NodeId(data[0].LOIPMes4.ToString());
                    DataValue LOIPVvalue4 = await _session.ReadValueAsync(LOIPVnode4);
                    _LineMgmtDetails.LOIP_Mes_Vcode4 = Convert.ToInt32(LOIPVvalue4.Value);

                    NodeId LOIPnode5 = new NodeId(data[0].LOIPSeq5.ToString());
                    DataValue LOIPvalue5 = await _session.ReadValueAsync(LOIPnode5);
                    _LineMgmtDetails.LOIP_Sequence5 = Convert.ToInt32(LOIPvalue5.Value);

                    NodeId LOIPVnode5 = new NodeId(data[0].LOIPMes5.ToString());
                    DataValue LOIPVvalue5 = await _session.ReadValueAsync(LOIPVnode5);
                    _LineMgmtDetails.LOIP_Mes_Vcode5 = Convert.ToInt32(LOIPVvalue5.Value);

                    NodeId LOIPnode6 = new NodeId(data[0].LOIPSeq6.ToString());
                    DataValue LOIPvalue6 = await _session.ReadValueAsync(LOIPnode6);
                    _LineMgmtDetails.LOIP_Sequence6 = Convert.ToInt32(LOIPvalue6.Value);

                    NodeId LOIPVnode6 = new NodeId(data[0].LOIPMes6.ToString());
                    DataValue LOIPVvalue6 = await _session.ReadValueAsync(LOIPVnode6);
                    _LineMgmtDetails.LOIP_Mes_Vcode6 = Convert.ToInt32(LOIPVvalue6.Value);

                    NodeId LOIPnode7 = new NodeId(data[0].LOIPSeq7.ToString());
                    DataValue LOIPvalue7 = await _session.ReadValueAsync(LOIPnode7);
                    _LineMgmtDetails.LOIP_Sequence7 = Convert.ToInt32(LOIPvalue7.Value);

                    NodeId LOIPVnode7 = new NodeId(data[0].LOIPMes7.ToString());
                    DataValue LOIPVvalue7 = await _session.ReadValueAsync(LOIPVnode7);
                    _LineMgmtDetails.LOIP_Mes_Vcode7 = Convert.ToInt32(LOIPVvalue7.Value);

                    NodeId LOIPnode8 = new NodeId(data[0].LOIPSeq8.ToString());
                    DataValue LOIPvalue8 = await _session.ReadValueAsync(LOIPnode8);
                    _LineMgmtDetails.LOIP_Sequence8 = Convert.ToInt32(LOIPvalue8.Value);

                    NodeId LOIPVnode8 = new NodeId(data[0].LOIPMes8.ToString());
                    DataValue LOIPVvalue8 = await _session.ReadValueAsync(LOIPVnode8);
                    _LineMgmtDetails.LOIP_Mes_Vcode8 = Convert.ToInt32(LOIPVvalue8.Value);

                    NodeId LOIPnode9 = new NodeId(data[0].LOIPSeq9.ToString());
                    DataValue LOIPvalue9 = await _session.ReadValueAsync(LOIPnode9);
                    _LineMgmtDetails.LOIP_Sequence9 = Convert.ToInt32(LOIPvalue9.Value);

                    NodeId LOIPVnode9 = new NodeId(data[0].LOIPMes9.ToString());
                    DataValue LOIPVvalue9 = await _session.ReadValueAsync(LOIPVnode9);
                    _LineMgmtDetails.LOIP_Mes_Vcode9 = Convert.ToInt32(LOIPVvalue9.Value);

                    NodeId LOIPnode10 = new NodeId(data[0].LOIPSeq10.ToString());
                    DataValue LOIPvalue10 = await _session.ReadValueAsync(LOIPnode10);
                    _LineMgmtDetails.LOIP_Sequence10 = Convert.ToInt32(LOIPvalue10.Value);

                    NodeId LOIPVnode10 = new NodeId(data[0].LOIPMes10.ToString());
                    DataValue LOIPVvalue10 = await _session.ReadValueAsync(LOIPVnode10);
                    _LineMgmtDetails.LOIP_Mes_Vcode10 = Convert.ToInt32(LOIPVvalue10.Value);

                    NodeId LOIPnode11 = new NodeId(data[0].LOIPSeq11.ToString());
                    DataValue LOIPvalue11 = await _session.ReadValueAsync(LOIPnode11);
                    _LineMgmtDetails.LOIP_Sequence11 = Convert.ToInt32(LOIPvalue11.Value);

                    NodeId LOIPVnode11 = new NodeId(data[0].LOIPMes11.ToString());
                    DataValue LOIPVvalue11 = await _session.ReadValueAsync(LOIPVnode11);
                    _LineMgmtDetails.LOIP_Mes_Vcode11 = Convert.ToInt32(LOIPVvalue11.Value);

                    NodeId LOIPnode12 = new NodeId(data[0].LOIPSeq12.ToString());
                    DataValue LOIPvalue12 = await _session.ReadValueAsync(LOIPnode12);
                    _LineMgmtDetails.LOIP_Sequence12 = Convert.ToInt32(LOIPvalue12.Value);

                    NodeId LOIPVnode12 = new NodeId(data[0].LOIPMes12.ToString());
                    DataValue LOIPVvalue12 = await _session.ReadValueAsync(LOIPVnode12);
                    _LineMgmtDetails.LOIP_Mes_Vcode12 = Convert.ToInt32(LOIPVvalue12.Value);

                    NodeId LOIPnode13 = new NodeId(data[0].LOIPSeq13.ToString());
                    DataValue LOIPvalue13 = await _session.ReadValueAsync(LOIPnode13);
                    _LineMgmtDetails.LOIP_Sequence13 = Convert.ToInt32(LOIPvalue13.Value);

                    NodeId LOIPVnode13 = new NodeId(data[0].LOIPMes13.ToString());
                    DataValue LOIPVvalue13 = await _session.ReadValueAsync(LOIPVnode13);
                    _LineMgmtDetails.LOIP_Mes_Vcode13 = Convert.ToInt32(LOIPVvalue13.Value);

                    NodeId LOIPnode14 = new NodeId(data[0].LOIPSeq14.ToString());
                    DataValue LOIPvalue14 = await _session.ReadValueAsync(LOIPnode14);
                    _LineMgmtDetails.LOIP_Sequence14 = Convert.ToInt32(LOIPvalue14.Value);

                    NodeId LOIPVnode14 = new NodeId(data[0].LOIPMes14.ToString());
                    DataValue LOIPVvalue14 = await _session.ReadValueAsync(LOIPVnode14);
                    _LineMgmtDetails.LOIP_Mes_Vcode14 = Convert.ToInt32(LOIPVvalue14.Value);

                    NodeId LOIPnode15 = new NodeId(data[0].LOIPSeq15.ToString());
                    DataValue LOIPvalue15 = await _session.ReadValueAsync(LOIPnode15);
                    _LineMgmtDetails.LOIP_Sequence15 = Convert.ToInt32(LOIPvalue15.Value);

                    NodeId LOIPVnode15 = new NodeId(data[0].LOIPMes15.ToString());
                    DataValue LOIPVvalue15 = await _session.ReadValueAsync(LOIPVnode15);
                    _LineMgmtDetails.LOIP_Mes_Vcode15 = Convert.ToInt32(LOIPVvalue15.Value);

                    NodeId LOIPnode16 = new NodeId(data[0].LOIPSeq16.ToString());
                    DataValue LOIPvalue16 = await _session.ReadValueAsync(LOIPnode16);
                    _LineMgmtDetails.LOIP_Sequence16 = Convert.ToInt32(LOIPvalue16.Value);

                    NodeId LOIPVnode16 = new NodeId(data[0].LOIPMes16.ToString());
                    DataValue LOIPVvalue16 = await _session.ReadValueAsync(LOIPVnode16);
                    _LineMgmtDetails.LOIP_Mes_Vcode16 = Convert.ToInt32(LOIPVvalue16.Value);

                    NodeId LOIPnode17 = new NodeId(data[0].LOIPSeq17.ToString());
                    DataValue LOIPvalue17 = await _session.ReadValueAsync(LOIPnode17);
                    _LineMgmtDetails.LOIP_Sequence17 = Convert.ToInt32(LOIPvalue17.Value);

                    NodeId LOIPVnode17 = new NodeId(data[0].LOIPMes17.ToString());
                    DataValue LOIPVvalue17 = await _session.ReadValueAsync(LOIPVnode17);
                    _LineMgmtDetails.LOIP_Mes_Vcode17 = Convert.ToInt32(LOIPVvalue17.Value);

                    NodeId LOIPnode18 = new NodeId(data[0].LOIPSeq18.ToString());
                    DataValue LOIPvalue18 = await _session.ReadValueAsync(LOIPnode18);
                    _LineMgmtDetails.LOIP_Sequence18 = Convert.ToInt32(LOIPvalue18.Value);

                    NodeId LOIPVnode18 = new NodeId(data[0].LOIPMes18.ToString());
                    DataValue LOIPVvalue18 = await _session.ReadValueAsync(LOIPVnode18);
                    _LineMgmtDetails.LOIP_Mes_Vcode18 = Convert.ToInt32(LOIPVvalue18.Value);

                    NodeId LOIPnode19 = new NodeId(data[0].LOIPSeq19.ToString());
                    DataValue LOIPvalue19 = await _session.ReadValueAsync(LOIPnode19);
                    _LineMgmtDetails.LOIP_Sequence19 = Convert.ToInt32(LOIPvalue19.Value);

                    NodeId LOIPVnode19 = new NodeId(data[0].LOIPMes19.ToString());
                    DataValue LOIPVvalue19 = await _session.ReadValueAsync(LOIPVnode19);
                    _LineMgmtDetails.LOIP_Mes_Vcode19 = Convert.ToInt32(LOIPVvalue19.Value);

                    NodeId LOIPnode20 = new NodeId(data[0].LOIPSeq20.ToString());
                    DataValue LOIPvalue20 = await _session.ReadValueAsync(LOIPnode20);
                    _LineMgmtDetails.LOIP_Sequence20 = Convert.ToInt32(LOIPvalue20.Value);

                    NodeId LOIPVnode20 = new NodeId(data[0].LOIPMes20.ToString());
                    DataValue LOIPVvalue20 = await _session.ReadValueAsync(LOIPVnode20);
                    _LineMgmtDetails.LOIP_Mes_Vcode20 = Convert.ToInt32(LOIPVvalue20.Value);

                    NodeId LOIPnode21 = new NodeId(data[0].LOIPSeq21.ToString());
                    DataValue LOIPvalue21 = await _session.ReadValueAsync(LOIPnode21);
                    _LineMgmtDetails.LOIP_Sequence21 = Convert.ToInt32(LOIPvalue21.Value);

                    NodeId LOIPVnode21 = new NodeId(data[0].LOIPMes21.ToString());
                    DataValue LOIPVvalue21 = await _session.ReadValueAsync(LOIPVnode21);
                    _LineMgmtDetails.LOIP_Mes_Vcode21 = Convert.ToInt32(LOIPVvalue21.Value);

                    NodeId LOIPnode22 = new NodeId(data[0].LOIPSeq22.ToString());
                    DataValue LOIPvalue22 = await _session.ReadValueAsync(LOIPnode22);
                    _LineMgmtDetails.LOIP_Sequence22 = Convert.ToInt32(LOIPvalue22.Value);

                    NodeId LOIPVnode22 = new NodeId(data[0].LOIPMes22.ToString());
                    DataValue LOIPVvalue22 = await _session.ReadValueAsync(LOIPVnode22);
                    _LineMgmtDetails.LOIP_Mes_Vcode22 = Convert.ToInt32(LOIPVvalue22.Value);

                    NodeId LOIPnode23 = new NodeId(data[0].LOIPSeq23.ToString());
                    DataValue LOIPvalue23 = await _session.ReadValueAsync(LOIPnode23);
                    _LineMgmtDetails.LOIP_Sequence23 = Convert.ToInt32(LOIPvalue23.Value);

                    NodeId LOIPVnode23 = new NodeId(data[0].LOIPMes23.ToString());
                    DataValue LOIPVvalue23 = await _session.ReadValueAsync(LOIPVnode23);
                    _LineMgmtDetails.LOIP_Mes_Vcode23 = Convert.ToInt32(LOIPVvalue23.Value);

                    NodeId LOIPnode24 = new NodeId(data[0].LOIPSeq24.ToString());
                    DataValue LOIPvalue24 = await _session.ReadValueAsync(LOIPnode24);
                    _LineMgmtDetails.LOIP_Sequence24 = Convert.ToInt32(LOIPvalue24.Value);

                    NodeId LOIPVnode24 = new NodeId(data[0].LOIPMes24.ToString());
                    DataValue LOIPVvalue24 = await _session.ReadValueAsync(LOIPVnode24);
                    _LineMgmtDetails.LOIP_Mes_Vcode24 = Convert.ToInt32(LOIPVvalue24.Value);

                    NodeId LOIPnode25 = new NodeId(data[0].LOIPSeq25.ToString());
                    DataValue LOIPvalue25 = await _session.ReadValueAsync(LOIPnode25);
                    _LineMgmtDetails.LOIP_Sequence25 = Convert.ToInt32(LOIPvalue25.Value);

                    NodeId LOIPVnode25 = new NodeId(data[0].LOIPMes25.ToString());
                    DataValue LOIPVvalue25 = await _session.ReadValueAsync(LOIPVnode25);
                    _LineMgmtDetails.LOIP_Mes_Vcode25 = Convert.ToInt32(LOIPVvalue25.Value);

                    NodeId LOIPnode26 = new NodeId(data[0].LOIPSeq26.ToString());
                    DataValue LOIPvalue26 = await _session.ReadValueAsync(LOIPnode26);
                    _LineMgmtDetails.LOIP_Sequence26 = Convert.ToInt32(LOIPvalue26.Value);

                    NodeId LOIPVnode26 = new NodeId(data[0].LOIPMes26.ToString());
                    DataValue LOIPVvalue26 = await _session.ReadValueAsync(LOIPVnode26);
                    _LineMgmtDetails.LOIP_Mes_Vcode26 = Convert.ToInt32(LOIPVvalue26.Value);

                    NodeId LOIPnode27 = new NodeId(data[0].LOIPSeq27.ToString());
                    DataValue LOIPvalue27 = await _session.ReadValueAsync(LOIPnode27);
                    _LineMgmtDetails.LOIP_Sequence27 = Convert.ToInt32(LOIPvalue27.Value);

                    NodeId LOIPVnode27 = new NodeId(data[0].LOIPMes27.ToString());
                    DataValue LOIPVvalue27 = await _session.ReadValueAsync(LOIPVnode27);
                    _LineMgmtDetails.LOIP_Mes_Vcode27 = Convert.ToInt32(LOIPVvalue27.Value);

                    NodeId LOIPnode28 = new NodeId(data[0].LOIPSeq28.ToString());
                    DataValue LOIPvalue28 = await _session.ReadValueAsync(LOIPnode28);
                    _LineMgmtDetails.LOIP_Sequence28 = Convert.ToInt32(LOIPvalue28.Value);

                    NodeId LOIPVnode28 = new NodeId(data[0].LOIPMes28.ToString());
                    DataValue LOIPVvalue28 = await _session.ReadValueAsync(LOIPVnode28);
                    _LineMgmtDetails.LOIP_Mes_Vcode28 = Convert.ToInt32(LOIPVvalue28.Value);

                    NodeId LOIPnode29 = new NodeId(data[0].LOIPSeq29.ToString());
                    DataValue LOIPvalue29 = await _session.ReadValueAsync(LOIPnode29);
                    _LineMgmtDetails.LOIP_Sequence29 = Convert.ToInt32(LOIPvalue29.Value);

                    NodeId LOIPVnode29 = new NodeId(data[0].LOIPMes29.ToString());
                    DataValue LOIPVvalue29 = await _session.ReadValueAsync(LOIPVnode29);
                    _LineMgmtDetails.LOIP_Mes_Vcode29 = Convert.ToInt32(LOIPVvalue29.Value);

                    NodeId LOIPnode30 = new NodeId(data[0].LOIPSeq30.ToString());
                    DataValue LOIPvalue30 = await _session.ReadValueAsync(LOIPnode30);
                    _LineMgmtDetails.LOIP_Sequence30 = Convert.ToInt32(LOIPvalue30.Value);

                    NodeId LOIPVnode30 = new NodeId(data[0].LOIPMes30.ToString());
                    DataValue LOIPVvalue30 = await _session.ReadValueAsync(LOIPVnode30);
                    _LineMgmtDetails.LOIP_Mes_Vcode30 = Convert.ToInt32(LOIPVvalue30.Value);
                }
                catch (Exception)
                {

                }
            }
        }
        else
        {
            //await ConnectAsync();
        }
        
        return _LineMgmtDetails;
    }
    public async Task<Line_Order_mgmt_Status_Details> ReadNodeLinemgmtFE()
    {
        Line_Order_mgmt_Status_Details _LineMgmtDetails = new Line_Order_mgmt_Status_Details();

        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    var data = DataVisulizationController.FE.ToList();

                    NodeId node1 = new NodeId(data[0].LOTSequence.ToString());
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _LineMgmtDetails.LOT_Sequence = Convert.ToInt32(value1.Value);

                    NodeId node2 = new NodeId(data[0].LOTMesVcode.ToString());
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _LineMgmtDetails.LOT_Mes_Vcode = Convert.ToInt32(value2.Value);

                    NodeId node3 = new NodeId(data[0].LOSSequence.ToString());
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _LineMgmtDetails.LOS_Sequence = Convert.ToInt32(value3.Value);

                    NodeId node4 = new NodeId(data[0].LOSMesVcode.ToString());
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _LineMgmtDetails.LOS_Mes_Vcode = Convert.ToInt32(value4.Value);

                    NodeId node5 = new NodeId(data[0].LOPSequence.ToString());
                    DataValue value5 = await _session.ReadValueAsync(node5);
                    _LineMgmtDetails.LOP_Sequence = Convert.ToInt32(value5.Value);

                    NodeId node6 = new NodeId(data[0].LOPMesVcode.ToString());
                    DataValue value6 = await _session.ReadValueAsync(node6);
                    _LineMgmtDetails.LOP_Mes_Vcode = Convert.ToInt32(value6.Value);

                    NodeId PRGnode1 = new NodeId(data[0].PRGSeq1.ToString());
                    DataValue PRGvalue1 = await _session.ReadValueAsync(PRGnode1);
                    _LineMgmtDetails.PRG_Sequence1 = Convert.ToInt32(PRGvalue1.Value);

                    NodeId PRGnode2 = new NodeId(data[0].PRGMes1.ToString());
                    DataValue PRGvalue2 = await _session.ReadValueAsync(PRGnode2);
                    _LineMgmtDetails.PRG_Mes_Vcode1 = Convert.ToInt32(PRGvalue2.Value);

                    NodeId PRGnode3 = new NodeId(data[0].PRGSeq2.ToString());
                    DataValue PRGvalue3 = await _session.ReadValueAsync(PRGnode3);
                    _LineMgmtDetails.PRG_Sequence2 = Convert.ToInt32(PRGvalue3.Value);

                    NodeId PRGnode4 = new NodeId(data[0].PRGMes2.ToString());
                    DataValue PRGvalue4 = await _session.ReadValueAsync(PRGnode4);
                    _LineMgmtDetails.PRG_Mes_Vcode2 = Convert.ToInt32(PRGvalue4.Value);

                    NodeId PRGnode5 = new NodeId(data[0].PRGSeq3.ToString());
                    DataValue PRGvalue5 = await _session.ReadValueAsync(PRGnode5);
                    _LineMgmtDetails.PRG_Sequence3 = Convert.ToInt32(PRGvalue5.Value);

                    NodeId PRGnode6 = new NodeId(data[0].PRGMes3.ToString());
                    DataValue PRGvalue6 = await _session.ReadValueAsync(PRGnode6);
                    _LineMgmtDetails.PRG_Mes_Vcode3 = Convert.ToInt32(PRGvalue6.Value);

                    NodeId PRGnode7 = new NodeId(data[0].PRGSeq4.ToString());
                    DataValue PRGvalue7 = await _session.ReadValueAsync(PRGnode7);
                    _LineMgmtDetails.PRG_Sequence4 = Convert.ToInt32(PRGvalue7.Value);

                    NodeId PRGnode8 = new NodeId(data[0].PRGMes4.ToString());
                    DataValue PRGvalue8 = await _session.ReadValueAsync(PRGnode8);
                    _LineMgmtDetails.PRG_Mes_Vcode4 = Convert.ToInt32(PRGvalue8.Value);

                    NodeId PRGnode9 = new NodeId(data[0].PRGSeq5.ToString());
                    DataValue PRGvalue9 = await _session.ReadValueAsync(PRGnode9);
                    _LineMgmtDetails.PRG_Sequence5 = Convert.ToInt32(PRGvalue9.Value);

                    NodeId PRGnode10 = new NodeId(data[0].PRGMes5.ToString());
                    DataValue PRGvalue10 = await _session.ReadValueAsync(PRGnode10);
                    _LineMgmtDetails.PRG_Mes_Vcode5 = Convert.ToInt32(PRGvalue10.Value);

                    NodeId LOIPnode1 = new NodeId(data[0].LOIPSeq1.ToString());
                    DataValue LOIPvalue1 = await _session.ReadValueAsync(LOIPnode1);
                    _LineMgmtDetails.LOIP_Sequence1 = Convert.ToInt32(LOIPvalue1.Value);

                    NodeId LOIPVnode1 = new NodeId(data[0].LOIPMes1.ToString());
                    DataValue LOIPVvalue1 = await _session.ReadValueAsync(LOIPVnode1);
                    _LineMgmtDetails.LOIP_Mes_Vcode1 = Convert.ToInt32(LOIPVvalue1.Value);

                    NodeId LOIPnode2 = new NodeId(data[0].LOIPSeq2.ToString());
                    DataValue LOIPvalue2 = await _session.ReadValueAsync(LOIPnode2);
                    _LineMgmtDetails.LOIP_Sequence2 = Convert.ToInt32(LOIPvalue2.Value);

                    NodeId LOIPVnode2 = new NodeId(data[0].LOIPMes2.ToString());
                    DataValue LOIPVvalue2 = await _session.ReadValueAsync(LOIPVnode2);
                    _LineMgmtDetails.LOIP_Mes_Vcode2 = Convert.ToInt32(LOIPVvalue2.Value);

                    NodeId LOIPnode3 = new NodeId(data[0].LOIPSeq3.ToString());
                    DataValue LOIPvalue3 = await _session.ReadValueAsync(LOIPnode3);
                    _LineMgmtDetails.LOIP_Sequence3 = Convert.ToInt32(LOIPvalue3.Value);

                    NodeId LOIPVnode3 = new NodeId(data[0].LOIPMes3.ToString());
                    DataValue LOIPVvalue3 = await _session.ReadValueAsync(LOIPVnode3);
                    _LineMgmtDetails.LOIP_Mes_Vcode3 = Convert.ToInt32(LOIPVvalue3.Value);

                    NodeId LOIPnode4 = new NodeId(data[0].LOIPSeq4.ToString());
                    DataValue LOIPvalue4 = await _session.ReadValueAsync(LOIPnode4);
                    _LineMgmtDetails.LOIP_Sequence4 = Convert.ToInt32(LOIPvalue4.Value);

                    NodeId LOIPVnode4 = new NodeId(data[0].LOIPMes4.ToString());
                    DataValue LOIPVvalue4 = await _session.ReadValueAsync(LOIPVnode4);
                    _LineMgmtDetails.LOIP_Mes_Vcode4 = Convert.ToInt32(LOIPVvalue4.Value);

                    NodeId LOIPnode5 = new NodeId(data[0].LOIPSeq5.ToString());
                    DataValue LOIPvalue5 = await _session.ReadValueAsync(LOIPnode5);
                    _LineMgmtDetails.LOIP_Sequence5 = Convert.ToInt32(LOIPvalue5.Value);

                    NodeId LOIPVnode5 = new NodeId(data[0].LOIPMes5.ToString());
                    DataValue LOIPVvalue5 = await _session.ReadValueAsync(LOIPVnode5);
                    _LineMgmtDetails.LOIP_Mes_Vcode5 = Convert.ToInt32(LOIPVvalue5.Value);

                    NodeId LOIPnode6 = new NodeId(data[0].LOIPSeq6.ToString());
                    DataValue LOIPvalue6 = await _session.ReadValueAsync(LOIPnode6);
                    _LineMgmtDetails.LOIP_Sequence6 = Convert.ToInt32(LOIPvalue6.Value);

                    NodeId LOIPVnode6 = new NodeId(data[0].LOIPMes6.ToString());
                    DataValue LOIPVvalue6 = await _session.ReadValueAsync(LOIPVnode6);
                    _LineMgmtDetails.LOIP_Mes_Vcode6 = Convert.ToInt32(LOIPVvalue6.Value);

                    NodeId LOIPnode7 = new NodeId(data[0].LOIPSeq7.ToString());
                    DataValue LOIPvalue7 = await _session.ReadValueAsync(LOIPnode7);
                    _LineMgmtDetails.LOIP_Sequence7 = Convert.ToInt32(LOIPvalue7.Value);

                    NodeId LOIPVnode7 = new NodeId(data[0].LOIPMes7.ToString());
                    DataValue LOIPVvalue7 = await _session.ReadValueAsync(LOIPVnode7);
                    _LineMgmtDetails.LOIP_Mes_Vcode7 = Convert.ToInt32(LOIPVvalue7.Value);

                    NodeId LOIPnode8 = new NodeId(data[0].LOIPSeq8.ToString());
                    DataValue LOIPvalue8 = await _session.ReadValueAsync(LOIPnode8);
                    _LineMgmtDetails.LOIP_Sequence8 = Convert.ToInt32(LOIPvalue8.Value);

                    NodeId LOIPVnode8 = new NodeId(data[0].LOIPMes8.ToString());
                    DataValue LOIPVvalue8 = await _session.ReadValueAsync(LOIPVnode8);
                    _LineMgmtDetails.LOIP_Mes_Vcode8 = Convert.ToInt32(LOIPVvalue8.Value);

                    NodeId LOIPnode9 = new NodeId(data[0].LOIPSeq9.ToString());
                    DataValue LOIPvalue9 = await _session.ReadValueAsync(LOIPnode9);
                    _LineMgmtDetails.LOIP_Sequence9 = Convert.ToInt32(LOIPvalue9.Value);

                    NodeId LOIPVnode9 = new NodeId(data[0].LOIPMes9.ToString());
                    DataValue LOIPVvalue9 = await _session.ReadValueAsync(LOIPVnode9);
                    _LineMgmtDetails.LOIP_Mes_Vcode9 = Convert.ToInt32(LOIPVvalue9.Value);

                    NodeId LOIPnode10 = new NodeId(data[0].LOIPSeq10.ToString());
                    DataValue LOIPvalue10 = await _session.ReadValueAsync(LOIPnode10);
                    _LineMgmtDetails.LOIP_Sequence10 = Convert.ToInt32(LOIPvalue10.Value);

                    NodeId LOIPVnode10 = new NodeId(data[0].LOIPMes10.ToString());
                    DataValue LOIPVvalue10 = await _session.ReadValueAsync(LOIPVnode10);
                    _LineMgmtDetails.LOIP_Mes_Vcode10 = Convert.ToInt32(LOIPVvalue10.Value);

                    NodeId LOIPnode11 = new NodeId(data[0].LOIPSeq11.ToString());
                    DataValue LOIPvalue11 = await _session.ReadValueAsync(LOIPnode11);
                    _LineMgmtDetails.LOIP_Sequence11 = Convert.ToInt32(LOIPvalue11.Value);

                    NodeId LOIPVnode11 = new NodeId(data[0].LOIPMes11.ToString());
                    DataValue LOIPVvalue11 = await _session.ReadValueAsync(LOIPVnode11);
                    _LineMgmtDetails.LOIP_Mes_Vcode11 = Convert.ToInt32(LOIPVvalue11.Value);

                    NodeId LOIPnode12 = new NodeId(data[0].LOIPSeq12.ToString());
                    DataValue LOIPvalue12 = await _session.ReadValueAsync(LOIPnode12);
                    _LineMgmtDetails.LOIP_Sequence12 = Convert.ToInt32(LOIPvalue12.Value);

                    NodeId LOIPVnode12 = new NodeId(data[0].LOIPMes12.ToString());
                    DataValue LOIPVvalue12 = await _session.ReadValueAsync(LOIPVnode12);
                    _LineMgmtDetails.LOIP_Mes_Vcode12 = Convert.ToInt32(LOIPVvalue12.Value);

                    NodeId LOIPnode13 = new NodeId(data[0].LOIPSeq13.ToString());
                    DataValue LOIPvalue13 = await _session.ReadValueAsync(LOIPnode13);
                    _LineMgmtDetails.LOIP_Sequence13 = Convert.ToInt32(LOIPvalue13.Value);

                    NodeId LOIPVnode13 = new NodeId(data[0].LOIPMes13.ToString());
                    DataValue LOIPVvalue13 = await _session.ReadValueAsync(LOIPVnode13);
                    _LineMgmtDetails.LOIP_Mes_Vcode13 = Convert.ToInt32(LOIPVvalue13.Value);

                    NodeId LOIPnode14 = new NodeId(data[0].LOIPSeq14.ToString());
                    DataValue LOIPvalue14 = await _session.ReadValueAsync(LOIPnode14);
                    _LineMgmtDetails.LOIP_Sequence14 = Convert.ToInt32(LOIPvalue14.Value);

                    NodeId LOIPVnode14 = new NodeId(data[0].LOIPMes14.ToString());
                    DataValue LOIPVvalue14 = await _session.ReadValueAsync(LOIPVnode14);
                    _LineMgmtDetails.LOIP_Mes_Vcode14 = Convert.ToInt32(LOIPVvalue14.Value);

                    NodeId LOIPnode15 = new NodeId(data[0].LOIPSeq15.ToString());
                    DataValue LOIPvalue15 = await _session.ReadValueAsync(LOIPnode15);
                    _LineMgmtDetails.LOIP_Sequence15 = Convert.ToInt32(LOIPvalue15.Value);

                    NodeId LOIPVnode15 = new NodeId(data[0].LOIPMes15.ToString());
                    DataValue LOIPVvalue15 = await _session.ReadValueAsync(LOIPVnode15);
                    _LineMgmtDetails.LOIP_Mes_Vcode15 = Convert.ToInt32(LOIPVvalue15.Value);

                    NodeId LOIPnode16 = new NodeId(data[0].LOIPSeq16.ToString());
                    DataValue LOIPvalue16 = await _session.ReadValueAsync(LOIPnode16);
                    _LineMgmtDetails.LOIP_Sequence16 = Convert.ToInt32(LOIPvalue16.Value);

                    NodeId LOIPVnode16 = new NodeId(data[0].LOIPMes16.ToString());
                    DataValue LOIPVvalue16 = await _session.ReadValueAsync(LOIPVnode16);
                    _LineMgmtDetails.LOIP_Mes_Vcode16 = Convert.ToInt32(LOIPVvalue16.Value);

                    NodeId LOIPnode17 = new NodeId(data[0].LOIPSeq17.ToString());
                    DataValue LOIPvalue17 = await _session.ReadValueAsync(LOIPnode17);
                    _LineMgmtDetails.LOIP_Sequence17 = Convert.ToInt32(LOIPvalue17.Value);

                    NodeId LOIPVnode17 = new NodeId(data[0].LOIPMes17.ToString());
                    DataValue LOIPVvalue17 = await _session.ReadValueAsync(LOIPVnode17);
                    _LineMgmtDetails.LOIP_Mes_Vcode17 = Convert.ToInt32(LOIPVvalue17.Value);

                    NodeId LOIPnode18 = new NodeId(data[0].LOIPSeq18.ToString());
                    DataValue LOIPvalue18 = await _session.ReadValueAsync(LOIPnode18);
                    _LineMgmtDetails.LOIP_Sequence18 = Convert.ToInt32(LOIPvalue18.Value);

                    NodeId LOIPVnode18 = new NodeId(data[0].LOIPMes18.ToString());
                    DataValue LOIPVvalue18 = await _session.ReadValueAsync(LOIPVnode18);
                    _LineMgmtDetails.LOIP_Mes_Vcode18 = Convert.ToInt32(LOIPVvalue18.Value);

                    NodeId LOIPnode19 = new NodeId(data[0].LOIPSeq19.ToString());
                    DataValue LOIPvalue19 = await _session.ReadValueAsync(LOIPnode19);
                    _LineMgmtDetails.LOIP_Sequence19 = Convert.ToInt32(LOIPvalue19.Value);

                    NodeId LOIPVnode19 = new NodeId(data[0].LOIPMes19.ToString());
                    DataValue LOIPVvalue19 = await _session.ReadValueAsync(LOIPVnode19);
                    _LineMgmtDetails.LOIP_Mes_Vcode19 = Convert.ToInt32(LOIPVvalue19.Value);

                    NodeId LOIPnode20 = new NodeId(data[0].LOIPSeq20.ToString());
                    DataValue LOIPvalue20 = await _session.ReadValueAsync(LOIPnode20);
                    _LineMgmtDetails.LOIP_Sequence20 = Convert.ToInt32(LOIPvalue20.Value);

                    NodeId LOIPVnode20 = new NodeId(data[0].LOIPMes20.ToString());
                    DataValue LOIPVvalue20 = await _session.ReadValueAsync(LOIPVnode20);
                    _LineMgmtDetails.LOIP_Mes_Vcode20 = Convert.ToInt32(LOIPVvalue20.Value);

                    NodeId LOIPnode21 = new NodeId(data[0].LOIPSeq21.ToString());
                    DataValue LOIPvalue21 = await _session.ReadValueAsync(LOIPnode21);
                    _LineMgmtDetails.LOIP_Sequence21 = Convert.ToInt32(LOIPvalue21.Value);

                    NodeId LOIPVnode21 = new NodeId(data[0].LOIPMes21.ToString());
                    DataValue LOIPVvalue21 = await _session.ReadValueAsync(LOIPVnode21);
                    _LineMgmtDetails.LOIP_Mes_Vcode21 = Convert.ToInt32(LOIPVvalue21.Value);

                    NodeId LOIPnode22 = new NodeId(data[0].LOIPSeq22.ToString());
                    DataValue LOIPvalue22 = await _session.ReadValueAsync(LOIPnode22);
                    _LineMgmtDetails.LOIP_Sequence22 = Convert.ToInt32(LOIPvalue22.Value);

                    NodeId LOIPVnode22 = new NodeId(data[0].LOIPMes22.ToString());
                    DataValue LOIPVvalue22 = await _session.ReadValueAsync(LOIPVnode22);
                    _LineMgmtDetails.LOIP_Mes_Vcode22 = Convert.ToInt32(LOIPVvalue22.Value);

                    NodeId LOIPnode23 = new NodeId(data[0].LOIPSeq23.ToString());
                    DataValue LOIPvalue23 = await _session.ReadValueAsync(LOIPnode23);
                    _LineMgmtDetails.LOIP_Sequence23 = Convert.ToInt32(LOIPvalue23.Value);

                    NodeId LOIPVnode23 = new NodeId(data[0].LOIPMes23.ToString());
                    DataValue LOIPVvalue23 = await _session.ReadValueAsync(LOIPVnode23);
                    _LineMgmtDetails.LOIP_Mes_Vcode23 = Convert.ToInt32(LOIPVvalue23.Value);

                    NodeId LOIPnode24 = new NodeId(data[0].LOIPSeq24.ToString());
                    DataValue LOIPvalue24 = await _session.ReadValueAsync(LOIPnode24);
                    _LineMgmtDetails.LOIP_Sequence24 = Convert.ToInt32(LOIPvalue24.Value);

                    NodeId LOIPVnode24 = new NodeId(data[0].LOIPMes24.ToString());
                    DataValue LOIPVvalue24 = await _session.ReadValueAsync(LOIPVnode24);
                    _LineMgmtDetails.LOIP_Mes_Vcode24 = Convert.ToInt32(LOIPVvalue24.Value);

                    NodeId LOIPnode25 = new NodeId(data[0].LOIPSeq25.ToString());
                    DataValue LOIPvalue25 = await _session.ReadValueAsync(LOIPnode25);
                    _LineMgmtDetails.LOIP_Sequence25 = Convert.ToInt32(LOIPvalue25.Value);

                    NodeId LOIPVnode25 = new NodeId(data[0].LOIPMes25.ToString());
                    DataValue LOIPVvalue25 = await _session.ReadValueAsync(LOIPVnode25);
                    _LineMgmtDetails.LOIP_Mes_Vcode25 = Convert.ToInt32(LOIPVvalue25.Value);

                    NodeId LOIPnode26 = new NodeId(data[0].LOIPSeq26.ToString());
                    DataValue LOIPvalue26 = await _session.ReadValueAsync(LOIPnode26);
                    _LineMgmtDetails.LOIP_Sequence26 = Convert.ToInt32(LOIPvalue26.Value);

                    NodeId LOIPVnode26 = new NodeId(data[0].LOIPMes26.ToString());
                    DataValue LOIPVvalue26 = await _session.ReadValueAsync(LOIPVnode26);
                    _LineMgmtDetails.LOIP_Mes_Vcode26 = Convert.ToInt32(LOIPVvalue26.Value);

                    NodeId LOIPnode27 = new NodeId(data[0].LOIPSeq27.ToString());
                    DataValue LOIPvalue27 = await _session.ReadValueAsync(LOIPnode27);
                    _LineMgmtDetails.LOIP_Sequence27 = Convert.ToInt32(LOIPvalue27.Value);

                    NodeId LOIPVnode27 = new NodeId(data[0].LOIPMes27.ToString());
                    DataValue LOIPVvalue27 = await _session.ReadValueAsync(LOIPVnode27);
                    _LineMgmtDetails.LOIP_Mes_Vcode27 = Convert.ToInt32(LOIPVvalue27.Value);

                    NodeId LOIPnode28 = new NodeId(data[0].LOIPSeq28.ToString());
                    DataValue LOIPvalue28 = await _session.ReadValueAsync(LOIPnode28);
                    _LineMgmtDetails.LOIP_Sequence28 = Convert.ToInt32(LOIPvalue28.Value);

                    NodeId LOIPVnode28 = new NodeId(data[0].LOIPMes28.ToString());
                    DataValue LOIPVvalue28 = await _session.ReadValueAsync(LOIPVnode28);
                    _LineMgmtDetails.LOIP_Mes_Vcode28 = Convert.ToInt32(LOIPVvalue28.Value);

                    NodeId LOIPnode29 = new NodeId(data[0].LOIPSeq29.ToString());
                    DataValue LOIPvalue29 = await _session.ReadValueAsync(LOIPnode29);
                    _LineMgmtDetails.LOIP_Sequence29 = Convert.ToInt32(LOIPvalue29.Value);

                    NodeId LOIPVnode29 = new NodeId(data[0].LOIPMes29.ToString());
                    DataValue LOIPVvalue29 = await _session.ReadValueAsync(LOIPVnode29);
                    _LineMgmtDetails.LOIP_Mes_Vcode29 = Convert.ToInt32(LOIPVvalue29.Value);

                    NodeId LOIPnode30 = new NodeId(data[0].LOIPSeq30.ToString());
                    DataValue LOIPvalue30 = await _session.ReadValueAsync(LOIPnode30);
                    _LineMgmtDetails.LOIP_Sequence30 = Convert.ToInt32(LOIPvalue30.Value);

                    NodeId LOIPVnode30 = new NodeId(data[0].LOIPMes30.ToString());
                    DataValue LOIPVvalue30 = await _session.ReadValueAsync(LOIPVnode30);
                    _LineMgmtDetails.LOIP_Mes_Vcode30 = Convert.ToInt32(LOIPVvalue30.Value);
                }
                catch (Exception)
                {

                }
            }
        }
        else
        {
            //await ConnectAsync();
        }

        return _LineMgmtDetails;
    }
    public async Task<Line_Order_mgmt_Status_Details> ReadNodeLinemgmtRF()
    {
        Line_Order_mgmt_Status_Details _LineMgmtDetails = new Line_Order_mgmt_Status_Details();

        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    var data = DataVisulizationController.RF.ToList();

                    NodeId node1 = new NodeId(data[0].LOTSequence.ToString());
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _LineMgmtDetails.LOT_Sequence = Convert.ToInt32(value1.Value);

                    NodeId node2 = new NodeId(data[0].LOTMesVcode.ToString());
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _LineMgmtDetails.LOT_Mes_Vcode = Convert.ToInt32(value2.Value);

                    NodeId node3 = new NodeId(data[0].LOSSequence.ToString());
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _LineMgmtDetails.LOS_Sequence = Convert.ToInt32(value3.Value);

                    NodeId node4 = new NodeId(data[0].LOSMesVcode.ToString());
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _LineMgmtDetails.LOS_Mes_Vcode = Convert.ToInt32(value4.Value);

                    NodeId node5 = new NodeId(data[0].LOPSequence.ToString());
                    DataValue value5 = await _session.ReadValueAsync(node5);
                    _LineMgmtDetails.LOP_Sequence = Convert.ToInt32(value5.Value);

                    NodeId node6 = new NodeId(data[0].LOPMesVcode.ToString());
                    DataValue value6 = await _session.ReadValueAsync(node6);
                    _LineMgmtDetails.LOP_Mes_Vcode = Convert.ToInt32(value6.Value);

                    NodeId PRGnode1 = new NodeId(data[0].PRGSeq1.ToString());
                    DataValue PRGvalue1 = await _session.ReadValueAsync(PRGnode1);
                    _LineMgmtDetails.PRG_Sequence1 = Convert.ToInt32(PRGvalue1.Value);

                    NodeId PRGnode2 = new NodeId(data[0].PRGMes1.ToString());
                    DataValue PRGvalue2 = await _session.ReadValueAsync(PRGnode2);
                    _LineMgmtDetails.PRG_Mes_Vcode1 = Convert.ToInt32(PRGvalue2.Value);

                    NodeId PRGnode3 = new NodeId(data[0].PRGSeq2.ToString());
                    DataValue PRGvalue3 = await _session.ReadValueAsync(PRGnode3);
                    _LineMgmtDetails.PRG_Sequence2 = Convert.ToInt32(PRGvalue3.Value);

                    NodeId PRGnode4 = new NodeId(data[0].PRGMes2.ToString());
                    DataValue PRGvalue4 = await _session.ReadValueAsync(PRGnode4);
                    _LineMgmtDetails.PRG_Mes_Vcode2 = Convert.ToInt32(PRGvalue4.Value);

                    NodeId PRGnode5 = new NodeId(data[0].PRGSeq3.ToString());
                    DataValue PRGvalue5 = await _session.ReadValueAsync(PRGnode5);
                    _LineMgmtDetails.PRG_Sequence3 = Convert.ToInt32(PRGvalue5.Value);

                    NodeId PRGnode6 = new NodeId(data[0].PRGMes3.ToString());
                    DataValue PRGvalue6 = await _session.ReadValueAsync(PRGnode6);
                    _LineMgmtDetails.PRG_Mes_Vcode3 = Convert.ToInt32(PRGvalue6.Value);

                    NodeId PRGnode7 = new NodeId(data[0].PRGSeq4.ToString());
                    DataValue PRGvalue7 = await _session.ReadValueAsync(PRGnode7);
                    _LineMgmtDetails.PRG_Sequence4 = Convert.ToInt32(PRGvalue7.Value);

                    NodeId PRGnode8 = new NodeId(data[0].PRGMes4.ToString());
                    DataValue PRGvalue8 = await _session.ReadValueAsync(PRGnode8);
                    _LineMgmtDetails.PRG_Mes_Vcode4 = Convert.ToInt32(PRGvalue8.Value);

                    NodeId PRGnode9 = new NodeId(data[0].PRGSeq5.ToString());
                    DataValue PRGvalue9 = await _session.ReadValueAsync(PRGnode9);
                    _LineMgmtDetails.PRG_Sequence5 = Convert.ToInt32(PRGvalue9.Value);

                    NodeId PRGnode10 = new NodeId(data[0].PRGMes5.ToString());
                    DataValue PRGvalue10 = await _session.ReadValueAsync(PRGnode10);
                    _LineMgmtDetails.PRG_Mes_Vcode5 = Convert.ToInt32(PRGvalue10.Value);

                    NodeId LOIPnode1 = new NodeId(data[0].LOIPSeq1.ToString());
                    DataValue LOIPvalue1 = await _session.ReadValueAsync(LOIPnode1);
                    _LineMgmtDetails.LOIP_Sequence1 = Convert.ToInt32(LOIPvalue1.Value);

                    NodeId LOIPVnode1 = new NodeId(data[0].LOIPMes1.ToString());
                    DataValue LOIPVvalue1 = await _session.ReadValueAsync(LOIPVnode1);
                    _LineMgmtDetails.LOIP_Mes_Vcode1 = Convert.ToInt32(LOIPVvalue1.Value);

                    NodeId LOIPnode2 = new NodeId(data[0].LOIPSeq2.ToString());
                    DataValue LOIPvalue2 = await _session.ReadValueAsync(LOIPnode2);
                    _LineMgmtDetails.LOIP_Sequence2 = Convert.ToInt32(LOIPvalue2.Value);

                    NodeId LOIPVnode2 = new NodeId(data[0].LOIPMes2.ToString());
                    DataValue LOIPVvalue2 = await _session.ReadValueAsync(LOIPVnode2);
                    _LineMgmtDetails.LOIP_Mes_Vcode2 = Convert.ToInt32(LOIPVvalue2.Value);

                    NodeId LOIPnode3 = new NodeId(data[0].LOIPSeq3.ToString());
                    DataValue LOIPvalue3 = await _session.ReadValueAsync(LOIPnode3);
                    _LineMgmtDetails.LOIP_Sequence3 = Convert.ToInt32(LOIPvalue3.Value);

                    NodeId LOIPVnode3 = new NodeId(data[0].LOIPMes3.ToString());
                    DataValue LOIPVvalue3 = await _session.ReadValueAsync(LOIPVnode3);
                    _LineMgmtDetails.LOIP_Mes_Vcode3 = Convert.ToInt32(LOIPVvalue3.Value);

                    NodeId LOIPnode4 = new NodeId(data[0].LOIPSeq4.ToString());
                    DataValue LOIPvalue4 = await _session.ReadValueAsync(LOIPnode4);
                    _LineMgmtDetails.LOIP_Sequence4 = Convert.ToInt32(LOIPvalue4.Value);

                    NodeId LOIPVnode4 = new NodeId(data[0].LOIPMes4.ToString());
                    DataValue LOIPVvalue4 = await _session.ReadValueAsync(LOIPVnode4);
                    _LineMgmtDetails.LOIP_Mes_Vcode4 = Convert.ToInt32(LOIPVvalue4.Value);

                    NodeId LOIPnode5 = new NodeId(data[0].LOIPSeq5.ToString());
                    DataValue LOIPvalue5 = await _session.ReadValueAsync(LOIPnode5);
                    _LineMgmtDetails.LOIP_Sequence5 = Convert.ToInt32(LOIPvalue5.Value);

                    NodeId LOIPVnode5 = new NodeId(data[0].LOIPMes5.ToString());
                    DataValue LOIPVvalue5 = await _session.ReadValueAsync(LOIPVnode5);
                    _LineMgmtDetails.LOIP_Mes_Vcode5 = Convert.ToInt32(LOIPVvalue5.Value);

                    NodeId LOIPnode6 = new NodeId(data[0].LOIPSeq6.ToString());
                    DataValue LOIPvalue6 = await _session.ReadValueAsync(LOIPnode6);
                    _LineMgmtDetails.LOIP_Sequence6 = Convert.ToInt32(LOIPvalue6.Value);

                    NodeId LOIPVnode6 = new NodeId(data[0].LOIPMes6.ToString());
                    DataValue LOIPVvalue6 = await _session.ReadValueAsync(LOIPVnode6);
                    _LineMgmtDetails.LOIP_Mes_Vcode6 = Convert.ToInt32(LOIPVvalue6.Value);

                    NodeId LOIPnode7 = new NodeId(data[0].LOIPSeq7.ToString());
                    DataValue LOIPvalue7 = await _session.ReadValueAsync(LOIPnode7);
                    _LineMgmtDetails.LOIP_Sequence7 = Convert.ToInt32(LOIPvalue7.Value);

                    NodeId LOIPVnode7 = new NodeId(data[0].LOIPMes7.ToString());
                    DataValue LOIPVvalue7 = await _session.ReadValueAsync(LOIPVnode7);
                    _LineMgmtDetails.LOIP_Mes_Vcode7 = Convert.ToInt32(LOIPVvalue7.Value);

                    NodeId LOIPnode8 = new NodeId(data[0].LOIPSeq8.ToString());
                    DataValue LOIPvalue8 = await _session.ReadValueAsync(LOIPnode8);
                    _LineMgmtDetails.LOIP_Sequence8 = Convert.ToInt32(LOIPvalue8.Value);

                    NodeId LOIPVnode8 = new NodeId(data[0].LOIPMes8.ToString());
                    DataValue LOIPVvalue8 = await _session.ReadValueAsync(LOIPVnode8);
                    _LineMgmtDetails.LOIP_Mes_Vcode8 = Convert.ToInt32(LOIPVvalue8.Value);

                    NodeId LOIPnode9 = new NodeId(data[0].LOIPSeq9.ToString());
                    DataValue LOIPvalue9 = await _session.ReadValueAsync(LOIPnode9);
                    _LineMgmtDetails.LOIP_Sequence9 = Convert.ToInt32(LOIPvalue9.Value);

                    NodeId LOIPVnode9 = new NodeId(data[0].LOIPMes9.ToString());
                    DataValue LOIPVvalue9 = await _session.ReadValueAsync(LOIPVnode9);
                    _LineMgmtDetails.LOIP_Mes_Vcode9 = Convert.ToInt32(LOIPVvalue9.Value);

                    NodeId LOIPnode10 = new NodeId(data[0].LOIPSeq10.ToString());
                    DataValue LOIPvalue10 = await _session.ReadValueAsync(LOIPnode10);
                    _LineMgmtDetails.LOIP_Sequence10 = Convert.ToInt32(LOIPvalue10.Value);

                    NodeId LOIPVnode10 = new NodeId(data[0].LOIPMes10.ToString());
                    DataValue LOIPVvalue10 = await _session.ReadValueAsync(LOIPVnode10);
                    _LineMgmtDetails.LOIP_Mes_Vcode10 = Convert.ToInt32(LOIPVvalue10.Value);

                    NodeId LOIPnode11 = new NodeId(data[0].LOIPSeq11.ToString());
                    DataValue LOIPvalue11 = await _session.ReadValueAsync(LOIPnode11);
                    _LineMgmtDetails.LOIP_Sequence11 = Convert.ToInt32(LOIPvalue11.Value);

                    NodeId LOIPVnode11 = new NodeId(data[0].LOIPMes11.ToString());
                    DataValue LOIPVvalue11 = await _session.ReadValueAsync(LOIPVnode11);
                    _LineMgmtDetails.LOIP_Mes_Vcode11 = Convert.ToInt32(LOIPVvalue11.Value);

                    NodeId LOIPnode12 = new NodeId(data[0].LOIPSeq12.ToString());
                    DataValue LOIPvalue12 = await _session.ReadValueAsync(LOIPnode12);
                    _LineMgmtDetails.LOIP_Sequence12 = Convert.ToInt32(LOIPvalue12.Value);

                    NodeId LOIPVnode12 = new NodeId(data[0].LOIPMes12.ToString());
                    DataValue LOIPVvalue12 = await _session.ReadValueAsync(LOIPVnode12);
                    _LineMgmtDetails.LOIP_Mes_Vcode12 = Convert.ToInt32(LOIPVvalue12.Value);

                    NodeId LOIPnode13 = new NodeId(data[0].LOIPSeq13.ToString());
                    DataValue LOIPvalue13 = await _session.ReadValueAsync(LOIPnode13);
                    _LineMgmtDetails.LOIP_Sequence13 = Convert.ToInt32(LOIPvalue13.Value);

                    NodeId LOIPVnode13 = new NodeId(data[0].LOIPMes13.ToString());
                    DataValue LOIPVvalue13 = await _session.ReadValueAsync(LOIPVnode13);
                    _LineMgmtDetails.LOIP_Mes_Vcode13 = Convert.ToInt32(LOIPVvalue13.Value);

                    NodeId LOIPnode14 = new NodeId(data[0].LOIPSeq14.ToString());
                    DataValue LOIPvalue14 = await _session.ReadValueAsync(LOIPnode14);
                    _LineMgmtDetails.LOIP_Sequence14 = Convert.ToInt32(LOIPvalue14.Value);

                    NodeId LOIPVnode14 = new NodeId(data[0].LOIPMes14.ToString());
                    DataValue LOIPVvalue14 = await _session.ReadValueAsync(LOIPVnode14);
                    _LineMgmtDetails.LOIP_Mes_Vcode14 = Convert.ToInt32(LOIPVvalue14.Value);

                    NodeId LOIPnode15 = new NodeId(data[0].LOIPSeq15.ToString());
                    DataValue LOIPvalue15 = await _session.ReadValueAsync(LOIPnode15);
                    _LineMgmtDetails.LOIP_Sequence15 = Convert.ToInt32(LOIPvalue15.Value);

                    NodeId LOIPVnode15 = new NodeId(data[0].LOIPMes15.ToString());
                    DataValue LOIPVvalue15 = await _session.ReadValueAsync(LOIPVnode15);
                    _LineMgmtDetails.LOIP_Mes_Vcode15 = Convert.ToInt32(LOIPVvalue15.Value);

                    NodeId LOIPnode16 = new NodeId(data[0].LOIPSeq16.ToString());
                    DataValue LOIPvalue16 = await _session.ReadValueAsync(LOIPnode16);
                    _LineMgmtDetails.LOIP_Sequence16 = Convert.ToInt32(LOIPvalue16.Value);

                    NodeId LOIPVnode16 = new NodeId(data[0].LOIPMes16.ToString());
                    DataValue LOIPVvalue16 = await _session.ReadValueAsync(LOIPVnode16);
                    _LineMgmtDetails.LOIP_Mes_Vcode16 = Convert.ToInt32(LOIPVvalue16.Value);

                    NodeId LOIPnode17 = new NodeId(data[0].LOIPSeq17.ToString());
                    DataValue LOIPvalue17 = await _session.ReadValueAsync(LOIPnode17);
                    _LineMgmtDetails.LOIP_Sequence17 = Convert.ToInt32(LOIPvalue17.Value);

                    NodeId LOIPVnode17 = new NodeId(data[0].LOIPMes17.ToString());
                    DataValue LOIPVvalue17 = await _session.ReadValueAsync(LOIPVnode17);
                    _LineMgmtDetails.LOIP_Mes_Vcode17 = Convert.ToInt32(LOIPVvalue17.Value);

                    NodeId LOIPnode18 = new NodeId(data[0].LOIPSeq18.ToString());
                    DataValue LOIPvalue18 = await _session.ReadValueAsync(LOIPnode18);
                    _LineMgmtDetails.LOIP_Sequence18 = Convert.ToInt32(LOIPvalue18.Value);

                    NodeId LOIPVnode18 = new NodeId(data[0].LOIPMes18.ToString());
                    DataValue LOIPVvalue18 = await _session.ReadValueAsync(LOIPVnode18);
                    _LineMgmtDetails.LOIP_Mes_Vcode18 = Convert.ToInt32(LOIPVvalue18.Value);

                    NodeId LOIPnode19 = new NodeId(data[0].LOIPSeq19.ToString());
                    DataValue LOIPvalue19 = await _session.ReadValueAsync(LOIPnode19);
                    _LineMgmtDetails.LOIP_Sequence19 = Convert.ToInt32(LOIPvalue19.Value);

                    NodeId LOIPVnode19 = new NodeId(data[0].LOIPMes19.ToString());
                    DataValue LOIPVvalue19 = await _session.ReadValueAsync(LOIPVnode19);
                    _LineMgmtDetails.LOIP_Mes_Vcode19 = Convert.ToInt32(LOIPVvalue19.Value);

                    NodeId LOIPnode20 = new NodeId(data[0].LOIPSeq20.ToString());
                    DataValue LOIPvalue20 = await _session.ReadValueAsync(LOIPnode20);
                    _LineMgmtDetails.LOIP_Sequence20 = Convert.ToInt32(LOIPvalue20.Value);

                    NodeId LOIPVnode20 = new NodeId(data[0].LOIPMes20.ToString());
                    DataValue LOIPVvalue20 = await _session.ReadValueAsync(LOIPVnode20);
                    _LineMgmtDetails.LOIP_Mes_Vcode20 = Convert.ToInt32(LOIPVvalue20.Value);

                    NodeId LOIPnode21 = new NodeId(data[0].LOIPSeq21.ToString());
                    DataValue LOIPvalue21 = await _session.ReadValueAsync(LOIPnode21);
                    _LineMgmtDetails.LOIP_Sequence21 = Convert.ToInt32(LOIPvalue21.Value);

                    NodeId LOIPVnode21 = new NodeId(data[0].LOIPMes21.ToString());
                    DataValue LOIPVvalue21 = await _session.ReadValueAsync(LOIPVnode21);
                    _LineMgmtDetails.LOIP_Mes_Vcode21 = Convert.ToInt32(LOIPVvalue21.Value);

                    NodeId LOIPnode22 = new NodeId(data[0].LOIPSeq22.ToString());
                    DataValue LOIPvalue22 = await _session.ReadValueAsync(LOIPnode22);
                    _LineMgmtDetails.LOIP_Sequence22 = Convert.ToInt32(LOIPvalue22.Value);

                    NodeId LOIPVnode22 = new NodeId(data[0].LOIPMes22.ToString());
                    DataValue LOIPVvalue22 = await _session.ReadValueAsync(LOIPVnode22);
                    _LineMgmtDetails.LOIP_Mes_Vcode22 = Convert.ToInt32(LOIPVvalue22.Value);

                    NodeId LOIPnode23 = new NodeId(data[0].LOIPSeq23.ToString());
                    DataValue LOIPvalue23 = await _session.ReadValueAsync(LOIPnode23);
                    _LineMgmtDetails.LOIP_Sequence23 = Convert.ToInt32(LOIPvalue23.Value);

                    NodeId LOIPVnode23 = new NodeId(data[0].LOIPMes23.ToString());
                    DataValue LOIPVvalue23 = await _session.ReadValueAsync(LOIPVnode23);
                    _LineMgmtDetails.LOIP_Mes_Vcode23 = Convert.ToInt32(LOIPVvalue23.Value);

                    NodeId LOIPnode24 = new NodeId(data[0].LOIPSeq24.ToString());
                    DataValue LOIPvalue24 = await _session.ReadValueAsync(LOIPnode24);
                    _LineMgmtDetails.LOIP_Sequence24 = Convert.ToInt32(LOIPvalue24.Value);

                    NodeId LOIPVnode24 = new NodeId(data[0].LOIPMes24.ToString());
                    DataValue LOIPVvalue24 = await _session.ReadValueAsync(LOIPVnode24);
                    _LineMgmtDetails.LOIP_Mes_Vcode24 = Convert.ToInt32(LOIPVvalue24.Value);

                    NodeId LOIPnode25 = new NodeId(data[0].LOIPSeq25.ToString());
                    DataValue LOIPvalue25 = await _session.ReadValueAsync(LOIPnode25);
                    _LineMgmtDetails.LOIP_Sequence25 = Convert.ToInt32(LOIPvalue25.Value);

                    NodeId LOIPVnode25 = new NodeId(data[0].LOIPMes25.ToString());
                    DataValue LOIPVvalue25 = await _session.ReadValueAsync(LOIPVnode25);
                    _LineMgmtDetails.LOIP_Mes_Vcode25 = Convert.ToInt32(LOIPVvalue25.Value);

                    NodeId LOIPnode26 = new NodeId(data[0].LOIPSeq26.ToString());
                    DataValue LOIPvalue26 = await _session.ReadValueAsync(LOIPnode26);
                    _LineMgmtDetails.LOIP_Sequence26 = Convert.ToInt32(LOIPvalue26.Value);

                    NodeId LOIPVnode26 = new NodeId(data[0].LOIPMes26.ToString());
                    DataValue LOIPVvalue26 = await _session.ReadValueAsync(LOIPVnode26);
                    _LineMgmtDetails.LOIP_Mes_Vcode26 = Convert.ToInt32(LOIPVvalue26.Value);

                    NodeId LOIPnode27 = new NodeId(data[0].LOIPSeq27.ToString());
                    DataValue LOIPvalue27 = await _session.ReadValueAsync(LOIPnode27);
                    _LineMgmtDetails.LOIP_Sequence27 = Convert.ToInt32(LOIPvalue27.Value);

                    NodeId LOIPVnode27 = new NodeId(data[0].LOIPMes27.ToString());
                    DataValue LOIPVvalue27 = await _session.ReadValueAsync(LOIPVnode27);
                    _LineMgmtDetails.LOIP_Mes_Vcode27 = Convert.ToInt32(LOIPVvalue27.Value);

                    NodeId LOIPnode28 = new NodeId(data[0].LOIPSeq28.ToString());
                    DataValue LOIPvalue28 = await _session.ReadValueAsync(LOIPnode28);
                    _LineMgmtDetails.LOIP_Sequence28 = Convert.ToInt32(LOIPvalue28.Value);

                    NodeId LOIPVnode28 = new NodeId(data[0].LOIPMes28.ToString());
                    DataValue LOIPVvalue28 = await _session.ReadValueAsync(LOIPVnode28);
                    _LineMgmtDetails.LOIP_Mes_Vcode28 = Convert.ToInt32(LOIPVvalue28.Value);

                    NodeId LOIPnode29 = new NodeId(data[0].LOIPSeq29.ToString());
                    DataValue LOIPvalue29 = await _session.ReadValueAsync(LOIPnode29);
                    _LineMgmtDetails.LOIP_Sequence29 = Convert.ToInt32(LOIPvalue29.Value);

                    NodeId LOIPVnode29 = new NodeId(data[0].LOIPMes29.ToString());
                    DataValue LOIPVvalue29 = await _session.ReadValueAsync(LOIPVnode29);
                    _LineMgmtDetails.LOIP_Mes_Vcode29 = Convert.ToInt32(LOIPVvalue29.Value);

                    NodeId LOIPnode30 = new NodeId(data[0].LOIPSeq30.ToString());
                    DataValue LOIPvalue30 = await _session.ReadValueAsync(LOIPnode30);
                    _LineMgmtDetails.LOIP_Sequence30 = Convert.ToInt32(LOIPvalue30.Value);

                    NodeId LOIPVnode30 = new NodeId(data[0].LOIPMes30.ToString());
                    DataValue LOIPVvalue30 = await _session.ReadValueAsync(LOIPVnode30);
                    _LineMgmtDetails.LOIP_Mes_Vcode30 = Convert.ToInt32(LOIPVvalue30.Value);
                }
                catch (Exception)
                {

                }
            }
        }
        else
        {
            //await ConnectAsync();
        }
        return _LineMgmtDetails;
    }
    public async Task<Line_Order_mgmt_Status_Details> ReadNodeLinemgmtBSRH()
    {
        Line_Order_mgmt_Status_Details _LineMgmtDetails = new Line_Order_mgmt_Status_Details();

        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    var data = DataVisulizationController.BSRH.ToList();

                    NodeId node1 = new NodeId(data[0].LOTSequence.ToString());
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _LineMgmtDetails.LOT_Sequence = Convert.ToInt32(value1.Value);

                    NodeId node2 = new NodeId(data[0].LOTMesVcode.ToString());
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _LineMgmtDetails.LOT_Mes_Vcode = Convert.ToInt32(value2.Value);

                    NodeId node3 = new NodeId(data[0].LOSSequence.ToString());
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _LineMgmtDetails.LOS_Sequence = Convert.ToInt32(value3.Value);

                    NodeId node4 = new NodeId(data[0].LOSMesVcode.ToString());
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _LineMgmtDetails.LOS_Mes_Vcode = Convert.ToInt32(value4.Value);

                    NodeId node5 = new NodeId(data[0].LOPSequence.ToString());
                    DataValue value5 = await _session.ReadValueAsync(node5);
                    _LineMgmtDetails.LOP_Sequence = Convert.ToInt32(value5.Value);

                    NodeId node6 = new NodeId(data[0].LOPMesVcode.ToString());
                    DataValue value6 = await _session.ReadValueAsync(node6);
                    _LineMgmtDetails.LOP_Mes_Vcode = Convert.ToInt32(value6.Value);

                    NodeId PRGnode1 = new NodeId(data[0].PRGSeq1.ToString());
                    DataValue PRGvalue1 = await _session.ReadValueAsync(PRGnode1);
                    _LineMgmtDetails.PRG_Sequence1 = Convert.ToInt32(PRGvalue1.Value);

                    NodeId PRGnode2 = new NodeId(data[0].PRGMes1.ToString());
                    DataValue PRGvalue2 = await _session.ReadValueAsync(PRGnode2);
                    _LineMgmtDetails.PRG_Mes_Vcode1 = Convert.ToInt32(PRGvalue2.Value);

                    NodeId PRGnode3 = new NodeId(data[0].PRGSeq2.ToString());
                    DataValue PRGvalue3 = await _session.ReadValueAsync(PRGnode3);
                    _LineMgmtDetails.PRG_Sequence2 = Convert.ToInt32(PRGvalue3.Value);

                    NodeId PRGnode4 = new NodeId(data[0].PRGMes2.ToString());
                    DataValue PRGvalue4 = await _session.ReadValueAsync(PRGnode4);
                    _LineMgmtDetails.PRG_Mes_Vcode2 = Convert.ToInt32(PRGvalue4.Value);

                    NodeId PRGnode5 = new NodeId(data[0].PRGSeq3.ToString());
                    DataValue PRGvalue5 = await _session.ReadValueAsync(PRGnode5);
                    _LineMgmtDetails.PRG_Sequence3 = Convert.ToInt32(PRGvalue5.Value);

                    NodeId PRGnode6 = new NodeId(data[0].PRGMes3.ToString());
                    DataValue PRGvalue6 = await _session.ReadValueAsync(PRGnode6);
                    _LineMgmtDetails.PRG_Mes_Vcode3 = Convert.ToInt32(PRGvalue6.Value);

                    NodeId PRGnode7 = new NodeId(data[0].PRGSeq4.ToString());
                    DataValue PRGvalue7 = await _session.ReadValueAsync(PRGnode7);
                    _LineMgmtDetails.PRG_Sequence4 = Convert.ToInt32(PRGvalue7.Value);

                    NodeId PRGnode8 = new NodeId(data[0].PRGMes4.ToString());
                    DataValue PRGvalue8 = await _session.ReadValueAsync(PRGnode8);
                    _LineMgmtDetails.PRG_Mes_Vcode4 = Convert.ToInt32(PRGvalue8.Value);

                    NodeId PRGnode9 = new NodeId(data[0].PRGSeq5.ToString());
                    DataValue PRGvalue9 = await _session.ReadValueAsync(PRGnode9);
                    _LineMgmtDetails.PRG_Sequence5 = Convert.ToInt32(PRGvalue9.Value);

                    NodeId PRGnode10 = new NodeId(data[0].PRGMes5.ToString());
                    DataValue PRGvalue10 = await _session.ReadValueAsync(PRGnode10);
                    _LineMgmtDetails.PRG_Mes_Vcode5 = Convert.ToInt32(PRGvalue10.Value);

                    NodeId LOIPnode1 = new NodeId(data[0].LOIPSeq1.ToString());
                    DataValue LOIPvalue1 = await _session.ReadValueAsync(LOIPnode1);
                    _LineMgmtDetails.LOIP_Sequence1 = Convert.ToInt32(LOIPvalue1.Value);

                    NodeId LOIPVnode1 = new NodeId(data[0].LOIPMes1.ToString());
                    DataValue LOIPVvalue1 = await _session.ReadValueAsync(LOIPVnode1);
                    _LineMgmtDetails.LOIP_Mes_Vcode1 = Convert.ToInt32(LOIPVvalue1.Value);

                    NodeId LOIPnode2 = new NodeId(data[0].LOIPSeq2.ToString());
                    DataValue LOIPvalue2 = await _session.ReadValueAsync(LOIPnode2);
                    _LineMgmtDetails.LOIP_Sequence2 = Convert.ToInt32(LOIPvalue2.Value);

                    NodeId LOIPVnode2 = new NodeId(data[0].LOIPMes2.ToString());
                    DataValue LOIPVvalue2 = await _session.ReadValueAsync(LOIPVnode2);
                    _LineMgmtDetails.LOIP_Mes_Vcode2 = Convert.ToInt32(LOIPVvalue2.Value);

                    NodeId LOIPnode3 = new NodeId(data[0].LOIPSeq3.ToString());
                    DataValue LOIPvalue3 = await _session.ReadValueAsync(LOIPnode3);
                    _LineMgmtDetails.LOIP_Sequence3 = Convert.ToInt32(LOIPvalue3.Value);

                    NodeId LOIPVnode3 = new NodeId(data[0].LOIPMes3.ToString());
                    DataValue LOIPVvalue3 = await _session.ReadValueAsync(LOIPVnode3);
                    _LineMgmtDetails.LOIP_Mes_Vcode3 = Convert.ToInt32(LOIPVvalue3.Value);

                    NodeId LOIPnode4 = new NodeId(data[0].LOIPSeq4.ToString());
                    DataValue LOIPvalue4 = await _session.ReadValueAsync(LOIPnode4);
                    _LineMgmtDetails.LOIP_Sequence4 = Convert.ToInt32(LOIPvalue4.Value);

                    NodeId LOIPVnode4 = new NodeId(data[0].LOIPMes4.ToString());
                    DataValue LOIPVvalue4 = await _session.ReadValueAsync(LOIPVnode4);
                    _LineMgmtDetails.LOIP_Mes_Vcode4 = Convert.ToInt32(LOIPVvalue4.Value);

                    NodeId LOIPnode5 = new NodeId(data[0].LOIPSeq5.ToString());
                    DataValue LOIPvalue5 = await _session.ReadValueAsync(LOIPnode5);
                    _LineMgmtDetails.LOIP_Sequence5 = Convert.ToInt32(LOIPvalue5.Value);

                    NodeId LOIPVnode5 = new NodeId(data[0].LOIPMes5.ToString());
                    DataValue LOIPVvalue5 = await _session.ReadValueAsync(LOIPVnode5);
                    _LineMgmtDetails.LOIP_Mes_Vcode5 = Convert.ToInt32(LOIPVvalue5.Value);

                    NodeId LOIPnode6 = new NodeId(data[0].LOIPSeq6.ToString());
                    DataValue LOIPvalue6 = await _session.ReadValueAsync(LOIPnode6);
                    _LineMgmtDetails.LOIP_Sequence6 = Convert.ToInt32(LOIPvalue6.Value);

                    NodeId LOIPVnode6 = new NodeId(data[0].LOIPMes6.ToString());
                    DataValue LOIPVvalue6 = await _session.ReadValueAsync(LOIPVnode6);
                    _LineMgmtDetails.LOIP_Mes_Vcode6 = Convert.ToInt32(LOIPVvalue6.Value);

                    NodeId LOIPnode7 = new NodeId(data[0].LOIPSeq7.ToString());
                    DataValue LOIPvalue7 = await _session.ReadValueAsync(LOIPnode7);
                    _LineMgmtDetails.LOIP_Sequence7 = Convert.ToInt32(LOIPvalue7.Value);

                    NodeId LOIPVnode7 = new NodeId(data[0].LOIPMes7.ToString());
                    DataValue LOIPVvalue7 = await _session.ReadValueAsync(LOIPVnode7);
                    _LineMgmtDetails.LOIP_Mes_Vcode7 = Convert.ToInt32(LOIPVvalue7.Value);

                    NodeId LOIPnode8 = new NodeId(data[0].LOIPSeq8.ToString());
                    DataValue LOIPvalue8 = await _session.ReadValueAsync(LOIPnode8);
                    _LineMgmtDetails.LOIP_Sequence8 = Convert.ToInt32(LOIPvalue8.Value);

                    NodeId LOIPVnode8 = new NodeId(data[0].LOIPMes8.ToString());
                    DataValue LOIPVvalue8 = await _session.ReadValueAsync(LOIPVnode8);
                    _LineMgmtDetails.LOIP_Mes_Vcode8 = Convert.ToInt32(LOIPVvalue8.Value);

                    NodeId LOIPnode9 = new NodeId(data[0].LOIPSeq9.ToString());
                    DataValue LOIPvalue9 = await _session.ReadValueAsync(LOIPnode9);
                    _LineMgmtDetails.LOIP_Sequence9 = Convert.ToInt32(LOIPvalue9.Value);

                    NodeId LOIPVnode9 = new NodeId(data[0].LOIPMes9.ToString());
                    DataValue LOIPVvalue9 = await _session.ReadValueAsync(LOIPVnode9);
                    _LineMgmtDetails.LOIP_Mes_Vcode9 = Convert.ToInt32(LOIPVvalue9.Value);

                    NodeId LOIPnode10 = new NodeId(data[0].LOIPSeq10.ToString());
                    DataValue LOIPvalue10 = await _session.ReadValueAsync(LOIPnode10);
                    _LineMgmtDetails.LOIP_Sequence10 = Convert.ToInt32(LOIPvalue10.Value);

                    NodeId LOIPVnode10 = new NodeId(data[0].LOIPMes10.ToString());
                    DataValue LOIPVvalue10 = await _session.ReadValueAsync(LOIPVnode10);
                    _LineMgmtDetails.LOIP_Mes_Vcode10 = Convert.ToInt32(LOIPVvalue10.Value);

                    NodeId LOIPnode11 = new NodeId(data[0].LOIPSeq11.ToString());
                    DataValue LOIPvalue11 = await _session.ReadValueAsync(LOIPnode11);
                    _LineMgmtDetails.LOIP_Sequence11 = Convert.ToInt32(LOIPvalue11.Value);

                    NodeId LOIPVnode11 = new NodeId(data[0].LOIPMes11.ToString());
                    DataValue LOIPVvalue11 = await _session.ReadValueAsync(LOIPVnode11);
                    _LineMgmtDetails.LOIP_Mes_Vcode11 = Convert.ToInt32(LOIPVvalue11.Value);

                    NodeId LOIPnode12 = new NodeId(data[0].LOIPSeq12.ToString());
                    DataValue LOIPvalue12 = await _session.ReadValueAsync(LOIPnode12);
                    _LineMgmtDetails.LOIP_Sequence12 = Convert.ToInt32(LOIPvalue12.Value);

                    NodeId LOIPVnode12 = new NodeId(data[0].LOIPMes12.ToString());
                    DataValue LOIPVvalue12 = await _session.ReadValueAsync(LOIPVnode12);
                    _LineMgmtDetails.LOIP_Mes_Vcode12 = Convert.ToInt32(LOIPVvalue12.Value);

                    NodeId LOIPnode13 = new NodeId(data[0].LOIPSeq13.ToString());
                    DataValue LOIPvalue13 = await _session.ReadValueAsync(LOIPnode13);
                    _LineMgmtDetails.LOIP_Sequence13 = Convert.ToInt32(LOIPvalue13.Value);

                    NodeId LOIPVnode13 = new NodeId(data[0].LOIPMes13.ToString());
                    DataValue LOIPVvalue13 = await _session.ReadValueAsync(LOIPVnode13);
                    _LineMgmtDetails.LOIP_Mes_Vcode13 = Convert.ToInt32(LOIPVvalue13.Value);

                    NodeId LOIPnode14 = new NodeId(data[0].LOIPSeq14.ToString());
                    DataValue LOIPvalue14 = await _session.ReadValueAsync(LOIPnode14);
                    _LineMgmtDetails.LOIP_Sequence14 = Convert.ToInt32(LOIPvalue14.Value);

                    NodeId LOIPVnode14 = new NodeId(data[0].LOIPMes14.ToString());
                    DataValue LOIPVvalue14 = await _session.ReadValueAsync(LOIPVnode14);
                    _LineMgmtDetails.LOIP_Mes_Vcode14 = Convert.ToInt32(LOIPVvalue14.Value);

                    NodeId LOIPnode15 = new NodeId(data[0].LOIPSeq15.ToString());
                    DataValue LOIPvalue15 = await _session.ReadValueAsync(LOIPnode15);
                    _LineMgmtDetails.LOIP_Sequence15 = Convert.ToInt32(LOIPvalue15.Value);

                    NodeId LOIPVnode15 = new NodeId(data[0].LOIPMes15.ToString());
                    DataValue LOIPVvalue15 = await _session.ReadValueAsync(LOIPVnode15);
                    _LineMgmtDetails.LOIP_Mes_Vcode15 = Convert.ToInt32(LOIPVvalue15.Value);

                    NodeId LOIPnode16 = new NodeId(data[0].LOIPSeq16.ToString());
                    DataValue LOIPvalue16 = await _session.ReadValueAsync(LOIPnode16);
                    _LineMgmtDetails.LOIP_Sequence16 = Convert.ToInt32(LOIPvalue16.Value);

                    NodeId LOIPVnode16 = new NodeId(data[0].LOIPMes16.ToString());
                    DataValue LOIPVvalue16 = await _session.ReadValueAsync(LOIPVnode16);
                    _LineMgmtDetails.LOIP_Mes_Vcode16 = Convert.ToInt32(LOIPVvalue16.Value);

                    NodeId LOIPnode17 = new NodeId(data[0].LOIPSeq17.ToString());
                    DataValue LOIPvalue17 = await _session.ReadValueAsync(LOIPnode17);
                    _LineMgmtDetails.LOIP_Sequence17 = Convert.ToInt32(LOIPvalue17.Value);

                    NodeId LOIPVnode17 = new NodeId(data[0].LOIPMes17.ToString());
                    DataValue LOIPVvalue17 = await _session.ReadValueAsync(LOIPVnode17);
                    _LineMgmtDetails.LOIP_Mes_Vcode17 = Convert.ToInt32(LOIPVvalue17.Value);

                    NodeId LOIPnode18 = new NodeId(data[0].LOIPSeq18.ToString());
                    DataValue LOIPvalue18 = await _session.ReadValueAsync(LOIPnode18);
                    _LineMgmtDetails.LOIP_Sequence18 = Convert.ToInt32(LOIPvalue18.Value);

                    NodeId LOIPVnode18 = new NodeId(data[0].LOIPMes18.ToString());
                    DataValue LOIPVvalue18 = await _session.ReadValueAsync(LOIPVnode18);
                    _LineMgmtDetails.LOIP_Mes_Vcode18 = Convert.ToInt32(LOIPVvalue18.Value);

                    NodeId LOIPnode19 = new NodeId(data[0].LOIPSeq19.ToString());
                    DataValue LOIPvalue19 = await _session.ReadValueAsync(LOIPnode19);
                    _LineMgmtDetails.LOIP_Sequence19 = Convert.ToInt32(LOIPvalue19.Value);

                    NodeId LOIPVnode19 = new NodeId(data[0].LOIPMes19.ToString());
                    DataValue LOIPVvalue19 = await _session.ReadValueAsync(LOIPVnode19);
                    _LineMgmtDetails.LOIP_Mes_Vcode19 = Convert.ToInt32(LOIPVvalue19.Value);

                    NodeId LOIPnode20 = new NodeId(data[0].LOIPSeq20.ToString());
                    DataValue LOIPvalue20 = await _session.ReadValueAsync(LOIPnode20);
                    _LineMgmtDetails.LOIP_Sequence20 = Convert.ToInt32(LOIPvalue20.Value);

                    NodeId LOIPVnode20 = new NodeId(data[0].LOIPMes20.ToString());
                    DataValue LOIPVvalue20 = await _session.ReadValueAsync(LOIPVnode20);
                    _LineMgmtDetails.LOIP_Mes_Vcode20 = Convert.ToInt32(LOIPVvalue20.Value);

                    NodeId LOIPnode21 = new NodeId(data[0].LOIPSeq21.ToString());
                    DataValue LOIPvalue21 = await _session.ReadValueAsync(LOIPnode21);
                    _LineMgmtDetails.LOIP_Sequence21 = Convert.ToInt32(LOIPvalue21.Value);

                    NodeId LOIPVnode21 = new NodeId(data[0].LOIPMes21.ToString());
                    DataValue LOIPVvalue21 = await _session.ReadValueAsync(LOIPVnode21);
                    _LineMgmtDetails.LOIP_Mes_Vcode21 = Convert.ToInt32(LOIPVvalue21.Value);

                    NodeId LOIPnode22 = new NodeId(data[0].LOIPSeq22.ToString());
                    DataValue LOIPvalue22 = await _session.ReadValueAsync(LOIPnode22);
                    _LineMgmtDetails.LOIP_Sequence22 = Convert.ToInt32(LOIPvalue22.Value);

                    NodeId LOIPVnode22 = new NodeId(data[0].LOIPMes22.ToString());
                    DataValue LOIPVvalue22 = await _session.ReadValueAsync(LOIPVnode22);
                    _LineMgmtDetails.LOIP_Mes_Vcode22 = Convert.ToInt32(LOIPVvalue22.Value);

                    NodeId LOIPnode23 = new NodeId(data[0].LOIPSeq23.ToString());
                    DataValue LOIPvalue23 = await _session.ReadValueAsync(LOIPnode23);
                    _LineMgmtDetails.LOIP_Sequence23 = Convert.ToInt32(LOIPvalue23.Value);

                    NodeId LOIPVnode23 = new NodeId(data[0].LOIPMes23.ToString());
                    DataValue LOIPVvalue23 = await _session.ReadValueAsync(LOIPVnode23);
                    _LineMgmtDetails.LOIP_Mes_Vcode23 = Convert.ToInt32(LOIPVvalue23.Value);

                    NodeId LOIPnode24 = new NodeId(data[0].LOIPSeq24.ToString());
                    DataValue LOIPvalue24 = await _session.ReadValueAsync(LOIPnode24);
                    _LineMgmtDetails.LOIP_Sequence24 = Convert.ToInt32(LOIPvalue24.Value);

                    NodeId LOIPVnode24 = new NodeId(data[0].LOIPMes24.ToString());
                    DataValue LOIPVvalue24 = await _session.ReadValueAsync(LOIPVnode24);
                    _LineMgmtDetails.LOIP_Mes_Vcode24 = Convert.ToInt32(LOIPVvalue24.Value);

                    NodeId LOIPnode25 = new NodeId(data[0].LOIPSeq25.ToString());
                    DataValue LOIPvalue25 = await _session.ReadValueAsync(LOIPnode25);
                    _LineMgmtDetails.LOIP_Sequence25 = Convert.ToInt32(LOIPvalue25.Value);

                    NodeId LOIPVnode25 = new NodeId(data[0].LOIPMes25.ToString());
                    DataValue LOIPVvalue25 = await _session.ReadValueAsync(LOIPVnode25);
                    _LineMgmtDetails.LOIP_Mes_Vcode25 = Convert.ToInt32(LOIPVvalue25.Value);

                    NodeId LOIPnode26 = new NodeId(data[0].LOIPSeq26.ToString());
                    DataValue LOIPvalue26 = await _session.ReadValueAsync(LOIPnode26);
                    _LineMgmtDetails.LOIP_Sequence26 = Convert.ToInt32(LOIPvalue26.Value);

                    NodeId LOIPVnode26 = new NodeId(data[0].LOIPMes26.ToString());
                    DataValue LOIPVvalue26 = await _session.ReadValueAsync(LOIPVnode26);
                    _LineMgmtDetails.LOIP_Mes_Vcode26 = Convert.ToInt32(LOIPVvalue26.Value);

                    NodeId LOIPnode27 = new NodeId(data[0].LOIPSeq27.ToString());
                    DataValue LOIPvalue27 = await _session.ReadValueAsync(LOIPnode27);
                    _LineMgmtDetails.LOIP_Sequence27 = Convert.ToInt32(LOIPvalue27.Value);

                    NodeId LOIPVnode27 = new NodeId(data[0].LOIPMes27.ToString());
                    DataValue LOIPVvalue27 = await _session.ReadValueAsync(LOIPVnode27);
                    _LineMgmtDetails.LOIP_Mes_Vcode27 = Convert.ToInt32(LOIPVvalue27.Value);

                    NodeId LOIPnode28 = new NodeId(data[0].LOIPSeq28.ToString());
                    DataValue LOIPvalue28 = await _session.ReadValueAsync(LOIPnode28);
                    _LineMgmtDetails.LOIP_Sequence28 = Convert.ToInt32(LOIPvalue28.Value);

                    NodeId LOIPVnode28 = new NodeId(data[0].LOIPMes28.ToString());
                    DataValue LOIPVvalue28 = await _session.ReadValueAsync(LOIPVnode28);
                    _LineMgmtDetails.LOIP_Mes_Vcode28 = Convert.ToInt32(LOIPVvalue28.Value);

                    NodeId LOIPnode29 = new NodeId(data[0].LOIPSeq29.ToString());
                    DataValue LOIPvalue29 = await _session.ReadValueAsync(LOIPnode29);
                    _LineMgmtDetails.LOIP_Sequence29 = Convert.ToInt32(LOIPvalue29.Value);

                    NodeId LOIPVnode29 = new NodeId(data[0].LOIPMes29.ToString());
                    DataValue LOIPVvalue29 = await _session.ReadValueAsync(LOIPVnode29);
                    _LineMgmtDetails.LOIP_Mes_Vcode29 = Convert.ToInt32(LOIPVvalue29.Value);

                    NodeId LOIPnode30 = new NodeId(data[0].LOIPSeq30.ToString());
                    DataValue LOIPvalue30 = await _session.ReadValueAsync(LOIPnode30);
                    _LineMgmtDetails.LOIP_Sequence30 = Convert.ToInt32(LOIPvalue30.Value);

                    NodeId LOIPVnode30 = new NodeId(data[0].LOIPMes30.ToString());
                    DataValue LOIPVvalue30 = await _session.ReadValueAsync(LOIPVnode30);
                    _LineMgmtDetails.LOIP_Mes_Vcode30 = Convert.ToInt32(LOIPVvalue30.Value);
                }
                catch (Exception)
                {                   

                }
            }
        }
        else
        {
            //await ConnectAsync();
        }
        return _LineMgmtDetails;
    }
    public async Task<Line_Order_mgmt_Status_Details> ReadNodeLinemgmtBSLH()
    {
        Line_Order_mgmt_Status_Details _LineMgmtDetails = new Line_Order_mgmt_Status_Details();

        if (_session != null)
        {
            if (_session.Connected)
            {
                try
                {
                    var data = DataVisulizationController.BSLH.ToList();

                    NodeId node1 = new NodeId(data[0].LOTSequence.ToString());
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _LineMgmtDetails.LOT_Sequence = Convert.ToInt32(value1.Value);

                    NodeId node2 = new NodeId(data[0].LOTMesVcode.ToString());
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _LineMgmtDetails.LOT_Mes_Vcode = Convert.ToInt32(value2.Value);

                    NodeId node3 = new NodeId(data[0].LOSSequence.ToString());
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _LineMgmtDetails.LOS_Sequence = Convert.ToInt32(value3.Value);

                    NodeId node4 = new NodeId(data[0].LOSMesVcode.ToString());
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _LineMgmtDetails.LOS_Mes_Vcode = Convert.ToInt32(value4.Value);

                    NodeId node5 = new NodeId(data[0].LOPSequence.ToString());
                    DataValue value5 = await _session.ReadValueAsync(node5);
                    _LineMgmtDetails.LOP_Sequence = Convert.ToInt32(value5.Value);

                    NodeId node6 = new NodeId(data[0].LOPMesVcode.ToString());
                    DataValue value6 = await _session.ReadValueAsync(node6);
                    _LineMgmtDetails.LOP_Mes_Vcode = Convert.ToInt32(value6.Value);

                    NodeId PRGnode1 = new NodeId(data[0].PRGSeq1.ToString());
                    DataValue PRGvalue1 = await _session.ReadValueAsync(PRGnode1);
                    _LineMgmtDetails.PRG_Sequence1 = Convert.ToInt32(PRGvalue1.Value);

                    NodeId PRGnode2 = new NodeId(data[0].PRGMes1.ToString());
                    DataValue PRGvalue2 = await _session.ReadValueAsync(PRGnode2);
                    _LineMgmtDetails.PRG_Mes_Vcode1 = Convert.ToInt32(PRGvalue2.Value);

                    NodeId PRGnode3 = new NodeId(data[0].PRGSeq2.ToString());
                    DataValue PRGvalue3 = await _session.ReadValueAsync(PRGnode3);
                    _LineMgmtDetails.PRG_Sequence2 = Convert.ToInt32(PRGvalue3.Value);

                    NodeId PRGnode4 = new NodeId(data[0].PRGMes2.ToString());
                    DataValue PRGvalue4 = await _session.ReadValueAsync(PRGnode4);
                    _LineMgmtDetails.PRG_Mes_Vcode2 = Convert.ToInt32(PRGvalue4.Value);

                    NodeId PRGnode5 = new NodeId(data[0].PRGSeq3.ToString());
                    DataValue PRGvalue5 = await _session.ReadValueAsync(PRGnode5);
                    _LineMgmtDetails.PRG_Sequence3 = Convert.ToInt32(PRGvalue5.Value);

                    NodeId PRGnode6 = new NodeId(data[0].PRGMes3.ToString());
                    DataValue PRGvalue6 = await _session.ReadValueAsync(PRGnode6);
                    _LineMgmtDetails.PRG_Mes_Vcode3 = Convert.ToInt32(PRGvalue6.Value);

                    NodeId PRGnode7 = new NodeId(data[0].PRGSeq4.ToString());
                    DataValue PRGvalue7 = await _session.ReadValueAsync(PRGnode7);
                    _LineMgmtDetails.PRG_Sequence4 = Convert.ToInt32(PRGvalue7.Value);

                    NodeId PRGnode8 = new NodeId(data[0].PRGMes4.ToString());
                    DataValue PRGvalue8 = await _session.ReadValueAsync(PRGnode8);
                    _LineMgmtDetails.PRG_Mes_Vcode4 = Convert.ToInt32(PRGvalue8.Value);

                    NodeId PRGnode9 = new NodeId(data[0].PRGSeq5.ToString());
                    DataValue PRGvalue9 = await _session.ReadValueAsync(PRGnode9);
                    _LineMgmtDetails.PRG_Sequence5 = Convert.ToInt32(PRGvalue9.Value);

                    NodeId PRGnode10 = new NodeId(data[0].PRGMes5.ToString());
                    DataValue PRGvalue10 = await _session.ReadValueAsync(PRGnode10);
                    _LineMgmtDetails.PRG_Mes_Vcode5 = Convert.ToInt32(PRGvalue10.Value);

                    NodeId LOIPnode1 = new NodeId(data[0].LOIPSeq1.ToString());
                    DataValue LOIPvalue1 = await _session.ReadValueAsync(LOIPnode1);
                    _LineMgmtDetails.LOIP_Sequence1 = Convert.ToInt32(LOIPvalue1.Value);

                    NodeId LOIPVnode1 = new NodeId(data[0].LOIPMes1.ToString());
                    DataValue LOIPVvalue1 = await _session.ReadValueAsync(LOIPVnode1);
                    _LineMgmtDetails.LOIP_Mes_Vcode1 = Convert.ToInt32(LOIPVvalue1.Value);

                    NodeId LOIPnode2 = new NodeId(data[0].LOIPSeq2.ToString());
                    DataValue LOIPvalue2 = await _session.ReadValueAsync(LOIPnode2);
                    _LineMgmtDetails.LOIP_Sequence2 = Convert.ToInt32(LOIPvalue2.Value);

                    NodeId LOIPVnode2 = new NodeId(data[0].LOIPMes2.ToString());
                    DataValue LOIPVvalue2 = await _session.ReadValueAsync(LOIPVnode2);
                    _LineMgmtDetails.LOIP_Mes_Vcode2 = Convert.ToInt32(LOIPVvalue2.Value);

                    NodeId LOIPnode3 = new NodeId(data[0].LOIPSeq3.ToString());
                    DataValue LOIPvalue3 = await _session.ReadValueAsync(LOIPnode3);
                    _LineMgmtDetails.LOIP_Sequence3 = Convert.ToInt32(LOIPvalue3.Value);

                    NodeId LOIPVnode3 = new NodeId(data[0].LOIPMes3.ToString());
                    DataValue LOIPVvalue3 = await _session.ReadValueAsync(LOIPVnode3);
                    _LineMgmtDetails.LOIP_Mes_Vcode3 = Convert.ToInt32(LOIPVvalue3.Value);

                    NodeId LOIPnode4 = new NodeId(data[0].LOIPSeq4.ToString());
                    DataValue LOIPvalue4 = await _session.ReadValueAsync(LOIPnode4);
                    _LineMgmtDetails.LOIP_Sequence4 = Convert.ToInt32(LOIPvalue4.Value);

                    NodeId LOIPVnode4 = new NodeId(data[0].LOIPMes4.ToString());
                    DataValue LOIPVvalue4 = await _session.ReadValueAsync(LOIPVnode4);
                    _LineMgmtDetails.LOIP_Mes_Vcode4 = Convert.ToInt32(LOIPVvalue4.Value);

                    NodeId LOIPnode5 = new NodeId(data[0].LOIPSeq5.ToString());
                    DataValue LOIPvalue5 = await _session.ReadValueAsync(LOIPnode5);
                    _LineMgmtDetails.LOIP_Sequence5 = Convert.ToInt32(LOIPvalue5.Value);

                    NodeId LOIPVnode5 = new NodeId(data[0].LOIPMes5.ToString());
                    DataValue LOIPVvalue5 = await _session.ReadValueAsync(LOIPVnode5);
                    _LineMgmtDetails.LOIP_Mes_Vcode5 = Convert.ToInt32(LOIPVvalue5.Value);

                    NodeId LOIPnode6 = new NodeId(data[0].LOIPSeq6.ToString());
                    DataValue LOIPvalue6 = await _session.ReadValueAsync(LOIPnode6);
                    _LineMgmtDetails.LOIP_Sequence6 = Convert.ToInt32(LOIPvalue6.Value);

                    NodeId LOIPVnode6 = new NodeId(data[0].LOIPMes6.ToString());
                    DataValue LOIPVvalue6 = await _session.ReadValueAsync(LOIPVnode6);
                    _LineMgmtDetails.LOIP_Mes_Vcode6 = Convert.ToInt32(LOIPVvalue6.Value);

                    NodeId LOIPnode7 = new NodeId(data[0].LOIPSeq7.ToString());
                    DataValue LOIPvalue7 = await _session.ReadValueAsync(LOIPnode7);
                    _LineMgmtDetails.LOIP_Sequence7 = Convert.ToInt32(LOIPvalue7.Value);

                    NodeId LOIPVnode7 = new NodeId(data[0].LOIPMes7.ToString());
                    DataValue LOIPVvalue7 = await _session.ReadValueAsync(LOIPVnode7);
                    _LineMgmtDetails.LOIP_Mes_Vcode7 = Convert.ToInt32(LOIPVvalue7.Value);

                    NodeId LOIPnode8 = new NodeId(data[0].LOIPSeq8.ToString());
                    DataValue LOIPvalue8 = await _session.ReadValueAsync(LOIPnode8);
                    _LineMgmtDetails.LOIP_Sequence8 = Convert.ToInt32(LOIPvalue8.Value);

                    NodeId LOIPVnode8 = new NodeId(data[0].LOIPMes8.ToString());
                    DataValue LOIPVvalue8 = await _session.ReadValueAsync(LOIPVnode8);
                    _LineMgmtDetails.LOIP_Mes_Vcode8 = Convert.ToInt32(LOIPVvalue8.Value);

                    NodeId LOIPnode9 = new NodeId(data[0].LOIPSeq9.ToString());
                    DataValue LOIPvalue9 = await _session.ReadValueAsync(LOIPnode9);
                    _LineMgmtDetails.LOIP_Sequence9 = Convert.ToInt32(LOIPvalue9.Value);

                    NodeId LOIPVnode9 = new NodeId(data[0].LOIPMes9.ToString());
                    DataValue LOIPVvalue9 = await _session.ReadValueAsync(LOIPVnode9);
                    _LineMgmtDetails.LOIP_Mes_Vcode9 = Convert.ToInt32(LOIPVvalue9.Value);

                    NodeId LOIPnode10 = new NodeId(data[0].LOIPSeq10.ToString());
                    DataValue LOIPvalue10 = await _session.ReadValueAsync(LOIPnode10);
                    _LineMgmtDetails.LOIP_Sequence10 = Convert.ToInt32(LOIPvalue10.Value);

                    NodeId LOIPVnode10 = new NodeId(data[0].LOIPMes10.ToString());
                    DataValue LOIPVvalue10 = await _session.ReadValueAsync(LOIPVnode10);
                    _LineMgmtDetails.LOIP_Mes_Vcode10 = Convert.ToInt32(LOIPVvalue10.Value);

                    NodeId LOIPnode11 = new NodeId(data[0].LOIPSeq11.ToString());
                    DataValue LOIPvalue11 = await _session.ReadValueAsync(LOIPnode11);
                    _LineMgmtDetails.LOIP_Sequence11 = Convert.ToInt32(LOIPvalue11.Value);

                    NodeId LOIPVnode11 = new NodeId(data[0].LOIPMes11.ToString());
                    DataValue LOIPVvalue11 = await _session.ReadValueAsync(LOIPVnode11);
                    _LineMgmtDetails.LOIP_Mes_Vcode11 = Convert.ToInt32(LOIPVvalue11.Value);

                    NodeId LOIPnode12 = new NodeId(data[0].LOIPSeq12.ToString());
                    DataValue LOIPvalue12 = await _session.ReadValueAsync(LOIPnode12);
                    _LineMgmtDetails.LOIP_Sequence12 = Convert.ToInt32(LOIPvalue12.Value);

                    NodeId LOIPVnode12 = new NodeId(data[0].LOIPMes12.ToString());
                    DataValue LOIPVvalue12 = await _session.ReadValueAsync(LOIPVnode12);
                    _LineMgmtDetails.LOIP_Mes_Vcode12 = Convert.ToInt32(LOIPVvalue12.Value);

                    NodeId LOIPnode13 = new NodeId(data[0].LOIPSeq13.ToString());
                    DataValue LOIPvalue13 = await _session.ReadValueAsync(LOIPnode13);
                    _LineMgmtDetails.LOIP_Sequence13 = Convert.ToInt32(LOIPvalue13.Value);

                    NodeId LOIPVnode13 = new NodeId(data[0].LOIPMes13.ToString());
                    DataValue LOIPVvalue13 = await _session.ReadValueAsync(LOIPVnode13);
                    _LineMgmtDetails.LOIP_Mes_Vcode13 = Convert.ToInt32(LOIPVvalue13.Value);

                    NodeId LOIPnode14 = new NodeId(data[0].LOIPSeq14.ToString());
                    DataValue LOIPvalue14 = await _session.ReadValueAsync(LOIPnode14);
                    _LineMgmtDetails.LOIP_Sequence14 = Convert.ToInt32(LOIPvalue14.Value);

                    NodeId LOIPVnode14 = new NodeId(data[0].LOIPMes14.ToString());
                    DataValue LOIPVvalue14 = await _session.ReadValueAsync(LOIPVnode14);
                    _LineMgmtDetails.LOIP_Mes_Vcode14 = Convert.ToInt32(LOIPVvalue14.Value);

                    NodeId LOIPnode15 = new NodeId(data[0].LOIPSeq15.ToString());
                    DataValue LOIPvalue15 = await _session.ReadValueAsync(LOIPnode15);
                    _LineMgmtDetails.LOIP_Sequence15 = Convert.ToInt32(LOIPvalue15.Value);

                    NodeId LOIPVnode15 = new NodeId(data[0].LOIPMes15.ToString());
                    DataValue LOIPVvalue15 = await _session.ReadValueAsync(LOIPVnode15);
                    _LineMgmtDetails.LOIP_Mes_Vcode15 = Convert.ToInt32(LOIPVvalue15.Value);

                    NodeId LOIPnode16 = new NodeId(data[0].LOIPSeq16.ToString());
                    DataValue LOIPvalue16 = await _session.ReadValueAsync(LOIPnode16);
                    _LineMgmtDetails.LOIP_Sequence16 = Convert.ToInt32(LOIPvalue16.Value);

                    NodeId LOIPVnode16 = new NodeId(data[0].LOIPMes16.ToString());
                    DataValue LOIPVvalue16 = await _session.ReadValueAsync(LOIPVnode16);
                    _LineMgmtDetails.LOIP_Mes_Vcode16 = Convert.ToInt32(LOIPVvalue16.Value);

                    NodeId LOIPnode17 = new NodeId(data[0].LOIPSeq17.ToString());
                    DataValue LOIPvalue17 = await _session.ReadValueAsync(LOIPnode17);
                    _LineMgmtDetails.LOIP_Sequence17 = Convert.ToInt32(LOIPvalue17.Value);

                    NodeId LOIPVnode17 = new NodeId(data[0].LOIPMes17.ToString());
                    DataValue LOIPVvalue17 = await _session.ReadValueAsync(LOIPVnode17);
                    _LineMgmtDetails.LOIP_Mes_Vcode17 = Convert.ToInt32(LOIPVvalue17.Value);

                    NodeId LOIPnode18 = new NodeId(data[0].LOIPSeq18.ToString());
                    DataValue LOIPvalue18 = await _session.ReadValueAsync(LOIPnode18);
                    _LineMgmtDetails.LOIP_Sequence18 = Convert.ToInt32(LOIPvalue18.Value);

                    NodeId LOIPVnode18 = new NodeId(data[0].LOIPMes18.ToString());
                    DataValue LOIPVvalue18 = await _session.ReadValueAsync(LOIPVnode18);
                    _LineMgmtDetails.LOIP_Mes_Vcode18 = Convert.ToInt32(LOIPVvalue18.Value);

                    NodeId LOIPnode19 = new NodeId(data[0].LOIPSeq19.ToString());
                    DataValue LOIPvalue19 = await _session.ReadValueAsync(LOIPnode19);
                    _LineMgmtDetails.LOIP_Sequence19 = Convert.ToInt32(LOIPvalue19.Value);

                    NodeId LOIPVnode19 = new NodeId(data[0].LOIPMes19.ToString());
                    DataValue LOIPVvalue19 = await _session.ReadValueAsync(LOIPVnode19);
                    _LineMgmtDetails.LOIP_Mes_Vcode19 = Convert.ToInt32(LOIPVvalue19.Value);

                    NodeId LOIPnode20 = new NodeId(data[0].LOIPSeq20.ToString());
                    DataValue LOIPvalue20 = await _session.ReadValueAsync(LOIPnode20);
                    _LineMgmtDetails.LOIP_Sequence20 = Convert.ToInt32(LOIPvalue20.Value);

                    NodeId LOIPVnode20 = new NodeId(data[0].LOIPMes20.ToString());
                    DataValue LOIPVvalue20 = await _session.ReadValueAsync(LOIPVnode20);
                    _LineMgmtDetails.LOIP_Mes_Vcode20 = Convert.ToInt32(LOIPVvalue20.Value);

                    NodeId LOIPnode21 = new NodeId(data[0].LOIPSeq21.ToString());
                    DataValue LOIPvalue21 = await _session.ReadValueAsync(LOIPnode21);
                    _LineMgmtDetails.LOIP_Sequence21 = Convert.ToInt32(LOIPvalue21.Value);

                    NodeId LOIPVnode21 = new NodeId(data[0].LOIPMes21.ToString());
                    DataValue LOIPVvalue21 = await _session.ReadValueAsync(LOIPVnode21);
                    _LineMgmtDetails.LOIP_Mes_Vcode21 = Convert.ToInt32(LOIPVvalue21.Value);

                    NodeId LOIPnode22 = new NodeId(data[0].LOIPSeq22.ToString());
                    DataValue LOIPvalue22 = await _session.ReadValueAsync(LOIPnode22);
                    _LineMgmtDetails.LOIP_Sequence22 = Convert.ToInt32(LOIPvalue22.Value);

                    NodeId LOIPVnode22 = new NodeId(data[0].LOIPMes22.ToString());
                    DataValue LOIPVvalue22 = await _session.ReadValueAsync(LOIPVnode22);
                    _LineMgmtDetails.LOIP_Mes_Vcode22 = Convert.ToInt32(LOIPVvalue22.Value);

                    NodeId LOIPnode23 = new NodeId(data[0].LOIPSeq23.ToString());
                    DataValue LOIPvalue23 = await _session.ReadValueAsync(LOIPnode23);
                    _LineMgmtDetails.LOIP_Sequence23 = Convert.ToInt32(LOIPvalue23.Value);

                    NodeId LOIPVnode23 = new NodeId(data[0].LOIPMes23.ToString());
                    DataValue LOIPVvalue23 = await _session.ReadValueAsync(LOIPVnode23);
                    _LineMgmtDetails.LOIP_Mes_Vcode23 = Convert.ToInt32(LOIPVvalue23.Value);

                    NodeId LOIPnode24 = new NodeId(data[0].LOIPSeq24.ToString());
                    DataValue LOIPvalue24 = await _session.ReadValueAsync(LOIPnode24);
                    _LineMgmtDetails.LOIP_Sequence24 = Convert.ToInt32(LOIPvalue24.Value);

                    NodeId LOIPVnode24 = new NodeId(data[0].LOIPMes24.ToString());
                    DataValue LOIPVvalue24 = await _session.ReadValueAsync(LOIPVnode24);
                    _LineMgmtDetails.LOIP_Mes_Vcode24 = Convert.ToInt32(LOIPVvalue24.Value);

                    NodeId LOIPnode25 = new NodeId(data[0].LOIPSeq25.ToString());
                    DataValue LOIPvalue25 = await _session.ReadValueAsync(LOIPnode25);
                    _LineMgmtDetails.LOIP_Sequence25 = Convert.ToInt32(LOIPvalue25.Value);

                    NodeId LOIPVnode25 = new NodeId(data[0].LOIPMes25.ToString());
                    DataValue LOIPVvalue25 = await _session.ReadValueAsync(LOIPVnode25);
                    _LineMgmtDetails.LOIP_Mes_Vcode25 = Convert.ToInt32(LOIPVvalue25.Value);

                    NodeId LOIPnode26 = new NodeId(data[0].LOIPSeq26.ToString());
                    DataValue LOIPvalue26 = await _session.ReadValueAsync(LOIPnode26);
                    _LineMgmtDetails.LOIP_Sequence26 = Convert.ToInt32(LOIPvalue26.Value);

                    NodeId LOIPVnode26 = new NodeId(data[0].LOIPMes26.ToString());
                    DataValue LOIPVvalue26 = await _session.ReadValueAsync(LOIPVnode26);
                    _LineMgmtDetails.LOIP_Mes_Vcode26 = Convert.ToInt32(LOIPVvalue26.Value);

                    NodeId LOIPnode27 = new NodeId(data[0].LOIPSeq27.ToString());
                    DataValue LOIPvalue27 = await _session.ReadValueAsync(LOIPnode27);
                    _LineMgmtDetails.LOIP_Sequence27 = Convert.ToInt32(LOIPvalue27.Value);

                    NodeId LOIPVnode27 = new NodeId(data[0].LOIPMes27.ToString());
                    DataValue LOIPVvalue27 = await _session.ReadValueAsync(LOIPVnode27);
                    _LineMgmtDetails.LOIP_Mes_Vcode27 = Convert.ToInt32(LOIPVvalue27.Value);

                    NodeId LOIPnode28 = new NodeId(data[0].LOIPSeq28.ToString());
                    DataValue LOIPvalue28 = await _session.ReadValueAsync(LOIPnode28);
                    _LineMgmtDetails.LOIP_Sequence28 = Convert.ToInt32(LOIPvalue28.Value);

                    NodeId LOIPVnode28 = new NodeId(data[0].LOIPMes28.ToString());
                    DataValue LOIPVvalue28 = await _session.ReadValueAsync(LOIPVnode28);
                    _LineMgmtDetails.LOIP_Mes_Vcode28 = Convert.ToInt32(LOIPVvalue28.Value);

                    NodeId LOIPnode29 = new NodeId(data[0].LOIPSeq29.ToString());
                    DataValue LOIPvalue29 = await _session.ReadValueAsync(LOIPnode29);
                    _LineMgmtDetails.LOIP_Sequence29 = Convert.ToInt32(LOIPvalue29.Value);

                    NodeId LOIPVnode29 = new NodeId(data[0].LOIPMes29.ToString());
                    DataValue LOIPVvalue29 = await _session.ReadValueAsync(LOIPVnode29);
                    _LineMgmtDetails.LOIP_Mes_Vcode29 = Convert.ToInt32(LOIPVvalue29.Value);

                    NodeId LOIPnode30 = new NodeId(data[0].LOIPSeq30.ToString());
                    DataValue LOIPvalue30 = await _session.ReadValueAsync(LOIPnode30);
                    _LineMgmtDetails.LOIP_Sequence30 = Convert.ToInt32(LOIPvalue30.Value);

                    NodeId LOIPVnode30 = new NodeId(data[0].LOIPMes30.ToString());
                    DataValue LOIPVvalue30 = await _session.ReadValueAsync(LOIPVnode30);
                    _LineMgmtDetails.LOIP_Mes_Vcode30 = Convert.ToInt32(LOIPVvalue30.Value);
                }
                catch (Exception ex)
                {
                    if(ex.Message.ToString()=="BadNotConnected")
                    {
                        await ConnectAsync();
                    }

                }
            }
        }
        else
        {
            //await ConnectAsync();
        }

        return _LineMgmtDetails;
    }

    public void Disconnect()
    {
        if (_session != null)
        {
            _session.Close();
            _session.Dispose();
            _session = null;
        }
    }
}



