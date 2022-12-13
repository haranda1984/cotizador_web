import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import Wrapper from "../Wrapper/Wrapper.component";
import TextField from "@material-ui/core/TextField";
import QuoteDetailsBox from "../QuoteDetailsBox";
import { InputAdornment } from "@material-ui/core";
import MuiAlert from "@material-ui/lab/Alert";
import Snackbar from "@material-ui/core/Snackbar";

function Alert(props) {
  return <MuiAlert elevation={6} variant="filled" {...props} />;
}

const Payments = ({ state, dispatch }) => {
  const [open, setOpen] = useState(false);
  const [openCondo, setOpenCondo] = useState(false);
  const { quote } = state;

  const handleClose = (event, reason) => {
    if (reason === "clickaway") {
      return;
    }
    setOpen(false);
    setOpenCondo(false);
  };

  const handleTextChange = ({ target: { value } }, field) => {
    value = value ? parseInt(value) : 0;

    if (value > 100) return;

    const updatedQuote = { ...quote };
    if (field === "monthlyPayments") {
      updatedQuote[field] = value;
    } else {
      updatedQuote[field] = (value / 100).toFixed(2);
    }

    let quoteDeposit = isNaN(updatedQuote.deposit) ? 0 : Math.trunc(updatedQuote.deposit * 100)
    let quoteLastPayment = isNaN(updatedQuote.lastPayment) ? 0 : Math.trunc(updatedQuote.lastPayment * 100)
    let quoteAdditionalPayments = isNaN(updatedQuote.additionalPayments) ? 0 : Math.trunc(updatedQuote.additionalPayments * 100)

    if (quoteDeposit + quoteAdditionalPayments + quoteLastPayment != 100) {
      setOpen(true);
    }


    dispatch({
      type: "SET_QUOTE",
      payload: {
        ...updatedQuote,
        "deposit-amount": updatedQuote.deposit * updatedQuote["final-price"],
        "monthlyPayments-amount":
          updatedQuote.monthlyPayments === 0
            ? 0
            : (updatedQuote.additionalPayments * updatedQuote["final-price"]) /
            updatedQuote.monthlyPayments,
        "additionalPayments-amount":
          updatedQuote.additionalPayments * updatedQuote["final-price"],
        "lastPayment-amount":
          updatedQuote.lastPayment * updatedQuote["final-price"],

      },
    });
  };

  useEffect(() => {
    dispatch({
      type: "SET_QUOTE",
      payload: {
        ...quote,
        "deposit-amount": quote.deposit * quote["final-price"],
        "monthlyPayments-amount":
          quote.monthlyPayments === 0
            ? 0
            : (quote.additionalPayments * quote["final-price"]) /
            quote.monthlyPayments,
        "additionalPayments-amount":
          quote.additionalPayments * quote["final-price"],
        "lastPayment-amount": quote.lastPayment * quote["final-price"],
        "condo-initial-payment": state.isCondo
          ? state.quote.unit.equipmentPrice * 0.1
          : 0,
        "condo-last-payment": state.isCondo
          ? state.quote.unit.equipmentPrice * 0.9
          : 0,
      },
    });
  }, []);

  return (
    <Wrapper
      style={{ flexFlow: "column", margin: "10px 0", overflow: "unset" }}
    >
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "1fr 1fr",
          gridGap: "0.5em",
        }}
      >
        <QuoteDetailsBox
          value={quote.unit.displayName}
          height="50px"
          title={"Nombre"}
          fontSize={22}
          isText
        />
        <QuoteDetailsBox
          value={quote["final-price"] ? quote["final-price"] : quote.unit.price}
          height="50px"
          title={"Precio final"}
          fontSize={22}
        />

        <TextField
          value={parseInt(quote.deposit * 100)}
          variant="outlined"
          label="Enganche"
          id="enganche"
          type="number"
          onChange={(e) => handleTextChange(e, "deposit")}
          className="no-spin"
          inputProps={{
            className: "no-spin",
            max: 100,
            min: 0,
          }}
          InputProps={{
            endAdornment: <InputAdornment position="end">%</InputAdornment>,
          }}
        />
        <QuoteDetailsBox
          value={quote["deposit-amount"]}
          height="100%"
          fontSize={22}
        />

        <TextField
          value={parseInt(quote.monthlyPayments)}
          variant="outlined"
          label="Mensualidades"
          id="Mensualidades"
          type="number"
          onChange={(e) => handleTextChange(e, "monthlyPayments")}
          className="no-spin"
          inputProps={{
            className: "no-spin",
            max: 100,
            min: 0,
          }}
        />
        <QuoteDetailsBox
          value={quote["monthlyPayments-amount"]}
          height="100%"
          fontSize={22}
        />
        <TextField
          value={parseInt(quote.additionalPayments * 100)}
          variant="outlined"
          label="Diferido"
          id="Diferido"
          type="number"
          onChange={(e) => handleTextChange(e, "additionalPayments")}
          className="no-spin"
          inputProps={{
            className: "no-spin",
            max: 100,
            min: 0,
          }}
          InputProps={{
            endAdornment: <InputAdornment position="end">%</InputAdornment>,
          }}
        />
        <QuoteDetailsBox
          value={quote["additionalPayments-amount"]}
          height="100%"
          fontSize={22}
        />
        <TextField
          value={parseInt(quote.lastPayment * 100)}
          variant="outlined"
          label="Contra entrega"
          id="contra"
          type="number"
          onChange={(e) => handleTextChange(e, "lastPayment")}
          className="no-spin"
          inputProps={{
            className: "no-spin",
            max: 100,
            min: 0,
          }}
          InputProps={{
            endAdornment: <InputAdornment position="end">%</InputAdornment>,
          }}
        />
        <QuoteDetailsBox
          value={quote["lastPayment-amount"]}
          height="100%"
          fontSize={22}
        />

        {state.isCondo && (
          <>
            <QuoteDetailsBox
              value={"Equipamiento"}
              height="50px"
              title={"Condo Hotel"}
              fontSize={22}
              isText
            />
            <QuoteDetailsBox
              value={quote.unit.equipmentPrice}
              height="50px"
              title={"Precio"}
              fontSize={22}
            />
          </>
        )}
      </div>
      <Snackbar open={open} autoHideDuration={6000} onClose={handleClose}>
        <Alert onClose={handleClose} severity="warning">
          La suma de enganche, monto diferido y contra entrega no puede ser
          diferente a 100%
        </Alert>
      </Snackbar>
      <Snackbar open={openCondo} autoHideDuration={6000} onClose={handleClose}>
        <Alert onClose={handleClose} severity="warning">
          La suma de pago inicial y 6 meses antes de entrega del Condo Hotel no
          puede ser diferente a 100%
        </Alert>
      </Snackbar>
    </Wrapper>
  );
};

export default Payments;

Payments.propTypes = {
  state: PropTypes.object.isRequired,
  dispatch: PropTypes.func.isRequired,
};
