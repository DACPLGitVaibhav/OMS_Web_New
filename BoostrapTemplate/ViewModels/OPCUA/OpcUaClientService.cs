using System;
using System.Linq;
using System.Threading.Tasks;
using DATA.Interaces;
using OMS_Template.ViewModels.OPCUA;
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

        if (_session!=null)
        {
            if (_session.Connected)
            {
                try
                {
                    _oPCUADetails.Isconnect = true;

                    NodeId node1 = new NodeId("ns=4;i=2221");
                    DataValue value1 = await _session.ReadValueAsync(node1);
                    _oPCUADetails.MPLC_FF = value1.Value.ToString();

                    NodeId node2 = new NodeId("ns=4;i=313");
                    DataValue value2 = await _session.ReadValueAsync(node2);
                    _oPCUADetails.MPLC_FE = value2.Value.ToString();

                    NodeId node3 = new NodeId("ns=4;i=313");
                    DataValue value3 = await _session.ReadValueAsync(node3);
                    _oPCUADetails.MPLC_RF = value3.Value.ToString();

                    NodeId node4 = new NodeId("ns=4;i=313");
                    DataValue value4 = await _session.ReadValueAsync(node4);
                    _oPCUADetails.MPLC_BSRH = value4.Value.ToString();

                    NodeId node5 = new NodeId("ns=4;i=313");
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

                  //  await ConnectAsync();
                }
            }
        }
        else
        {
            _oPCUADetails = null;
        }
      


        return _oPCUADetails;
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



