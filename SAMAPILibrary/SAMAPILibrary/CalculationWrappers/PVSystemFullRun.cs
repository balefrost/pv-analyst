﻿using SAMAPILibrary.DataHandling.OutputData;
using SAMAPILibrary.DataHandling.Parameters;
using SAMAPILibrary.DataHandling.Parameters.InverterModels;

namespace SAMAPILibrary.CalculationWrappers
{
    public class PVSystemFullRun
    {
        public static CompiledOutputData run(GISData gis, GUIData gui)
        {

            var pvo = runPVSS(gis);
            var uro = runUtilityRate(gui, pvo);
            var sc = runSizeAndCostSettings(gui, pvo);
            var clo = runCashLoan(gui, sc, uro);

            return new CompiledOutputData(pvo, uro, clo, sc);
        }

        private static CashLoanOutput runCashLoan(GUIData gui, SizeAndCostSettings sc, UtilityRateOutput uro)
        {
            CashLoanSettings cls = CashLoanSettings.getDefault();
            GUIAdapter.applyCashLoanSettings(cls, gui);
            cls.setValuesFromPriorOutput(sc, uro);
            CashLoanOutput clo = (CashLoanOutput) ModuleRunner.runModule(cls);
            return clo;
        }

        private static SizeAndCostSettings runSizeAndCostSettings(GUIData gui, PVSAMV1Output pvo)
        {
            SizeAndCostSettings sc = SizeAndCostSettings.getDefault();
            GUIAdapter.applySizeAndCostSettings(sc, gui);
            sc.setValuesFromPriorOutput(pvo);
            return sc;
        }

        private static UtilityRateOutput runUtilityRate(GUIData gui, PVSAMV1Output pvo)
        {
            UtilityRateSettings urs = UtilityRateSettings.getDefault();
            GUIAdapter.applyUtilityRateSettings(urs, gui);
            urs.setValuesFromPriorOutput(pvo);
            UtilityRateOutput uro = (UtilityRateOutput) ModuleRunner.runModule(urs);
            return uro;
        }

        private static PVSAMV1Output runPVSS(GISData gis)
        {
            PVSAMV1Settings pvss = PVSAMV1Settings.getDefault();
            GISAdapter.applySettings(pvss, gis);
            float arraypower = pvss.modules_per_string*pvss.strings_in_parallel*pvss.module_model.getRatedPower();
            DatasheetInverterSettings inverter = new DatasheetInverterSettings("default", arraypower*1.15f);
            pvss.inverter_model = inverter;
            PVSAMV1Output pvo = (PVSAMV1Output) ModuleRunner.runModule(pvss);
            return pvo;
        }
    }
}
