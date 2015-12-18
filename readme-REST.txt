Add to Hosts file
127.0.0.1 Digst.OioIdws.Rest.AS
127.0.0.1 Digst.OioIdws.Rest.wsp

install cert "REST AS SSL.pfx" into "LocalMachine/My" and "LocalMachine/Trust"
netsh http add sslcert ipport=0.0.0.0:10001 certhash=F194C2379F8DEF480FF310B785829254136CA8AE appid={00000000-0000-0000-0000-000000000000}

install cert "REST WSP SSL.pfx" into "LocalMachine/My" and "LocalMachine/Trust"
netsh http add sslcert ipport=0.0.0.0:10002 certhash=F0549886736E2F726F96D38D86EF4B65A8A6B2D1 appid={00000000-0000-0000-0000-000000000000}
