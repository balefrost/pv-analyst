﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMAPILibrary.DataHandling.Parameters;
using System.IO;

namespace SAMAPILibrary.CalculationWrappers
{
    public class GUITranslator
    {

        public static MultiplePVSystemModel multiRunModel(GISData gis, GUIData gui, int[] years)
        {
            MultiplePVSystemModel pv = new MultiplePVSystemModel();
            for (int i = 0; i < years.Length; i++)
            {
                String fn = "ExampleFiles\\Prospector\\radwx_075153995_" + years[i] + "~.tm2";
                pv.Add(runModel(gis, gui, fn));
            }

            return pv;
        }

        public static PVSystemModel runModel(GISData gis, GUIData gui)
        {
            return runModel(gis, gui, null);
        }

        public static PVSystemModel runModel(GISData gis, GUIData gui,string filename)
        {
            

            ArrayParameterListBuilder ab = new ArrayParameterListBuilder();
            UtilityRateParameterBuilder ub = new UtilityRateParameterBuilder();
            CashLoanParameterBuilder cb = new CashLoanParameterBuilder();
            SizeAndCostParameterBuilder sc = new SizeAndCostParameterBuilder();

            if (filename != null)
            {
                ab.weather_file(filename);
            }

            sc.overall_cost_per_watt_dc(gui.cost_per_watt_dc);

            ub.analysis_years(gui.analysis_years);
            ub.rate_escalation(gui.inflation_rate + gui.utility_ann_escal_rate);
            ub.ur_monthly_fixed_charge(gui.utility_monthly_fixed_cost);
            ub.ur_flat_buy_rate(gui.utility_price_to_compare);
            ub.ur_flat_sell_rate(gui.utility_price_to_compare);
            ub.ur_sell_eq_buy(true);

            cb.loan_rate(gui.loan_rate);
            cb.loan_term(gui.loan_term);
            cb.loan_debt(gui.loan_debt);
            cb.inflation_rate(gui.inflation_rate);
            cb.discount_rate(gui.discount_rate);

            if (gui.enable_incentives)
            {//Enable
                cb.pbi_sta_amount(gui.srec_price / 1000f);
                cb.itc_fed_percent(30);
            }
            else
            {//Disable
                cb.pbi_sta_amount(0);
                cb.itc_fed_percent(0);
            }

            PVSystemModel pv = new PVSystemModel(gis, ab, ub, cb, sc);
            pv.run();

            return pv;
        }

        
    }
}
