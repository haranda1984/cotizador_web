import React, { useEffect } from "react";

import { PRIMARYBLUE } from "../../constants/utils";
import { useLocation, navigate } from "@reach/router";
import * as moment from "moment";
import axios from "axios";
import endpoints from "../../api-service/endpoints";

import InverlevyStyledWrapper from "../../components/InvyWrapper/InvyWrapper.component";
import StyledPaper from "../../components/Paper/Paper.component";
import InverLevyStepper from "../../components/InvyStepper/InvyStepper.component";
import Unit from "../../components/Quote/Unit/Unit.component";
import QuoteHeader from "../QuoteHeader/QuoteHeader.component";
import Policy from "../../components/Quote/Policy/Policy.component";
import Payments from "../../components/Quote/Payments/Payments.component";
import FixedPayments from "../../components/Quote/FixedPayments/FixedPayments.component";
import CapitalGain from "../../components/Quote/CapitalGain/CapitalGain.component";


const initialState = {
  isCapitalGainLoading: true,
  isLoading: true,
  hasError: false,
  step: 1,
  totalSteps: 6,
  isCondo: false,
  currency: "MXN",
};

const reducer = (state, action) => {
  switch (action.type) {
    case "SET_IS_CAPITALGAIN_LOADING": {
      const { payload } = action;
      return { ...state, isCapitalGainLoading: payload };
    }
    case "SET_IS_LOADING": {
      const { payload } = action;
      return { ...state, isLoading: payload };
    }
    case "SET_CURRENCY": {
      const { payload } = action;
      return { ...state, currency: payload, quote: { ...state.quote, unit: undefined } };
    }
    case "SET_HAS_ERROR": {
      const { payload } = action;
      return { ...state, hasError: payload };
    }
    case "SET_STEP": {
      const { payload } = action;
      return { ...state, step: payload };
    }
    case "SET_IS_CONDO": {
      const { payload } = action;
      return { ...state, ...payload };
    }
    case "SET_QUOTE": {
      const { payload } = action;
      return { ...state, quote: { ...state.quote, ...payload } };
    }
    case "REPLACE_QUOTE": {
      const { payload } = action;
      return { ...state, quote: payload };
    }
    default:
      throw new Error();
  }
};

const Quote = () => {
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  const [state, dispatch] = React.useReducer(reducer, {
    ...initialState,
    quote: {
      product: params.get("product"),
      "initial-investment": "",
      "initial-date": moment().format("yyyy-MM-DD"),
      units: [],
      tradepolicies: [],
      "additions-payments": [],
      "manual-additions": [],
      discount: 0,
    },
  });


  useEffect(() => {
    const loadProduct = async () => {
      const { data } = await axios.get(
        `${endpoints.PROJECT}/${state.quote.product}/units?currency=${state.currency}`
      );
      dispatch({ type: "SET_QUOTE", payload: { units: data } });
      dispatch({ type: "SET_IS_LOADING", payload: false });
    };
    loadProduct();
  }, [state.currency]);

  let componentStep = null;
  let headerStep = null;

  switch (state.step) {
    case 1:
      componentStep = <Unit state={state} dispatch={dispatch} />;
      headerStep = (
        <QuoteHeader
          title="Seleccione: Unidad"
          onNext={() => dispatch({ type: "SET_STEP", payload: state.step + 1 })}
          disabled={!state.quote.unit}
          state={state}
          dispatch={dispatch}
        />
      );
      break;
    case 2:
      componentStep = <Policy state={state} dispatch={dispatch} />;
      headerStep = (
        <QuoteHeader
          title="Seleccione: Política Comercial"
          onNext={() => dispatch({ type: "SET_STEP", payload: state.step + 1 })}
          onPrevious={() =>
            dispatch({ type: "SET_STEP", payload: state.step - 1 })
          }
          state={state}
          dispatch={dispatch}
          disabled={!state.quote.tradepolicy}
        />
      );
      break;
    case 3:
      componentStep = <Payments state={state} dispatch={dispatch} />;
      headerStep = (
        <QuoteHeader
          title="Desglose de Pagos"
          onNext={() => dispatch({ type: "SET_STEP", payload: state.step + 1 })}
          onPrevious={() =>
            dispatch({ type: "SET_STEP", payload: state.step - 1 })
          }
          disabled={paymentsAreValid()}
          state={state}
          dispatch={dispatch}
        />
      );
      break;
    case 4:
      componentStep = <FixedPayments state={state} dispatch={dispatch} />;
      headerStep = (
        <QuoteHeader
          title="Pagos Diferidos"
          onNext={() => dispatch({ type: "SET_STEP", payload: state.step + 1 })}
          onPrevious={() =>
            dispatch({ type: "SET_STEP", payload: state.step - 1 })
          }
          state={state}
          dispatch={dispatch}
        />
      );
      break;
    case 5:
      componentStep = <CapitalGain dispatch={dispatch} state={state} />;
        headerStep = (
          <QuoteHeader
            lastStep={true}
            disabled={state.hasError | state.isCapitalGainLoading}
            title="Capitalización"
            onNext={() => navigate("quote-details", { state: { ...state.quote, isCondo: state.isCondo, currency: state.currency } })}
            onPrevious={() =>
              dispatch({ type: "SET_STEP", payload: state.step - 1 })
            }
            state={state}
            dispatch={dispatch}
          />
        );
      break;
    default:
      componentStep = <span>Error</span>;
      break;
  }

  return (
    <InverlevyStyledWrapper>
      {headerStep}
      <StyledPaper>{componentStep}</StyledPaper>
      <div
        style={{
          backgroundColor: "white",
          width: "70%",
          display: "flex",
          justifyContent: "flex-end",
        }}
      >
        <div
          style={{
            paddingRight: "0.5em",
            paddingBottom: "0.5em",
            color: PRIMARYBLUE,
          }}
        >
          {" "}
          <span style={{ fontWeight: "bolder" }}>
            {`${state.step} / ${state.totalSteps - 1}`}{" "}
          </span>
        </div>
      </div>
      <div style={{ width: "70%" }}>
        <InverLevyStepper step={state.step} steps={state.totalSteps} />
      </div>
    </InverlevyStyledWrapper>
  );
};

export default Quote;
