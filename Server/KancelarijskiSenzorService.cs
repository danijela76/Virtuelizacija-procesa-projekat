using Common;
using System;
using System.ServiceModel;

namespace Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class KancelarijskiSenzorService : IKancelarijskiSenzorService
    {
        private bool sessionActive = false;
        private SessionMeta currentSessionMeta = null;

        public OperationResponse StartSession(SessionMeta meta)
        {
            ValidateSessionMeta(meta);

            sessionActive = true;
            currentSessionMeta = meta;

            Console.WriteLine($"[SERVER] Sesija pokrenuta. {meta}");

            return OperationResponse.Ack("IN_PROGRESS", "Sesija uspesno pokrenuta.");
        }

        public OperationResponse PushSample(SensorSample sample)
        {
            if (!sessionActive)
            {
                return OperationResponse.Nack("FAILED", "Nema aktivne sesije. Pozovite StartSession pre slanja uzoraka.");
            }

            ValidateSample(sample);

            Console.WriteLine($"[SERVER] Uzorak primljen: {sample}");

            return OperationResponse.Ack("IN_PROGRESS", "Uzorak uspesno primljen.");
        }

        public OperationResponse EndSession()
        {
            if (!sessionActive)
            {
                return OperationResponse.Nack("FAILED", "Nema aktivne sesije.");
            }

            sessionActive = false;
            currentSessionMeta = null;

            Console.WriteLine("[SERVER] Sesija zavrsena.");

            return OperationResponse.Ack("COMPLETED", "Sesija uspesno zavrsena.");
        }

        private void ValidateSessionMeta(SessionMeta meta)
        {
            if (meta == null)
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("Meta zaglavlje sesije ne sme biti null.", "meta"),
                    new FaultReason("Nevazece meta zaglavlje."));
            }

            if (meta.DateTime == default(DateTime))
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("DateTime polje nije postavljeno.", "DateTime"),
                    new FaultReason("Nevazeci datum i vreme u meta zaglavlju."));
            }

            if (meta.Pressure <= 0 || meta.Pressure > 1100)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Pritisak mora biti u opsegu 0 do 1100 hPa.", "Pressure", "0 < Pressure <= 1100"),
                    new FaultReason("Nevazeca vrednost pritiska u meta zaglavlju."));
            }

            if (meta.Volume < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Jacina zvuka ne moze biti negativna.", "Volume", ">= 0"),
                    new FaultReason("Nevazeca vrednost jacine zvuka u meta zaglavlju."));
            }

            if (meta.TemperatureDHT < -40 || meta.TemperatureDHT > 80)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Temperatura DHT senzora van dozvoljenog opsega.", "TemperatureDHT", "-40 do 80 °C"),
                    new FaultReason("Nevazeca vrednost temperature DHT u meta zaglavlju."));
            }

            if (meta.TemperatureBMP < -40 || meta.TemperatureBMP > 85)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Temperatura BMP senzora van dozvoljenog opsega.", "TemperatureBMP", "-40 do 85 °C"),
                    new FaultReason("Nevazeca vrednost temperature BMP u meta zaglavlju."));
            }
        }

        private void ValidateSample(SensorSample sample)
        {
            if (sample == null)
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("Uzorak ne sme biti null.", "sample"),
                    new FaultReason("Nevazeci uzorak."));
            }

            if (sample.DateTime == default(DateTime))
            {
                throw new FaultException<DataFormatFault>(
                    new DataFormatFault("DateTime polje uzorka nije postavljeno.", "DateTime"),
                    new FaultReason("Nevazeci datum i vreme u uzorku."));
            }

            if (sample.Pressure <= 0 || sample.Pressure > 1100)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Pritisak mora biti u opsegu 0 do 1100 hPa.", "Pressure", "0 < Pressure <= 1100"),
                    new FaultReason("Nevazeca vrednost pritiska u uzorku."));
            }

            if (sample.Volume < 0)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Jacina zvuka ne moze biti negativna.", "Volume", ">= 0"),
                    new FaultReason("Nevazeca vrednost jacine zvuka u uzorku."));
            }

            if (sample.TemperatureDHT < -40 || sample.TemperatureDHT > 80)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Temperatura DHT senzora van dozvoljenog opsega.", "TemperatureDHT", "-40 do 80 °C"),
                    new FaultReason("Nevazeca vrednost temperature DHT u uzorku."));
            }

            if (sample.TemperatureBMP < -40 || sample.TemperatureBMP > 85)
            {
                throw new FaultException<ValidationFault>(
                    new ValidationFault("Temperatura BMP senzora van dozvoljenog opsega.", "TemperatureBMP", "-40 do 85 °C"),
                    new FaultReason("Nevazeca vrednost temperature BMP u uzorku."));
            }
        }
    }
}