// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  servicesUrl: "http://epyhost:5000/",
  TipoLogTiempoServiceUrl:"http://epyhost:7000/",
  CuotaDeTrabajoUrl:"http://epyhost:6000/",
  authServiceUrl:"https://epyhost:6001/v1/account/ExternalLogin?provider=aad&returnUrl=",
};
