export interface QRCodeDTO{
  image: string;
}
export class QRCode{
  constructor(image: string){
    this.image = image;
  }
  image: string;
}

export interface SetupQRCodeDTO{
  manualSetupKey: string;
  image: string;
}

export class SetupQRCode{
  constructor(manualSetupKey: string, image: string){
    this.manualSetupKey = manualSetupKey;
    this.image = image;
  }
  manualSetupKey: string;
  image: string;
}

export interface ValidationSetupResultDTO{
  isSuccess: boolean;
}

export class ValidationSetupResult{
  constructor(isSuccess: boolean){
    this.isSuccess = isSuccess;
  }
  isSuccess: boolean;
}
