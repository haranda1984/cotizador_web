import React, { useEffect } from "react";
import * as moment from "moment";
import { v4 as uuidv4 } from "uuid";
import styled from "styled-components";
import PropTypes from "prop-types";
import Wrapper from "../Wrapper/Wrapper.component";
import { TextField } from "@material-ui/core";
import { InverlevyButton } from "../../InvyButton/InvyButton.component";
import { MONEYFORMATTER, PRIMARYCOLOR } from "../../../constants/utils";
import QuoteDetailsBox from "../QuoteDetailsBox";
import { NumberFormatCustom } from "../Unit/Unit.component";

const StyledSeparator = styled.hr`
  border: "1px solid ${PRIMARYCOLOR}";
`;

const StyledWrapper = styled.div`
  display: grid;
  grid-template-columns: 2fr 2fr 1fr;
  grid-gap: 1em;
  width: 100%;
  margin-top: 15px;
  overflow: auto;
`;

const StyledManualOperationWrapper = styled.div`
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: ${PRIMARYCOLOR};
  color: white;
  border-radius: 5px;
`;
const minDate = moment().add(5, "d").format("yyyy-MM-DD");

const FixedPayments = ({ state, dispatch }) => {
  const { quote } = state;
  const [operation, setOperation] = React.useState({
    date: "",
    amount: "",
    id: "",
  });

  const updatePayments = (modifiedQuote) => {
    let total = operation.amount;
    let payments = [...modifiedQuote["additions-payments"]];
    total += modifiedQuote["manual-additions"].reduce(
      (accumulated, current) => (accumulated += current.amount),
      0
    );
    let subtotal = modifiedQuote["additionalPayments-amount"] - total;
    modifiedQuote["additionalPayments-amount"] = subtotal;
    modifiedQuote["total-manual-additions"] = total;
    modifiedQuote["manual-additions"] = [
      ...modifiedQuote["manual-additions"],
      { ...operation, id: uuidv4() },
    ];
    const monthlyAmount = +(
      modifiedQuote["additionalPayments-amount"] / modifiedQuote.monthlyPayments
    ).toFixed(2);
    payments.forEach((p) => {
      p.amount = monthlyAmount;
    });
    modifiedQuote["additions-payments"] = payments;

    dispatch({ type: "REPLACE_QUOTE", payload: modifiedQuote });
  };

  const handleClick = () => {
    const modifiedQuote = { ...quote };
    updatePayments(modifiedQuote);
    setOperation({ date: "", amount: "" });
  };

  const handleTextChange = (e, id) => {
    const amount = e.target.value !== "" ? parseFloat(e.target.value) : 0;
  };

  const handleDateChange = (e, id) => {
    const editedPayments = [...quote["additions-payments"]].map((x) => {
      if (x.id === id) {
        x.date = e.target.value;
      }
      return x;
    });
    dispatch({
      type: "SET_QUOTE",
      payload: { "additions-payments": editedPayments },
    });
  };

  const calculateMontlyPayments = () => {
    const modifiedQuote = { ...quote };
    let total = 0;
    let payments = [];
    total += modifiedQuote["manual-additions"].reduce(
      (accumulated, current) => (accumulated += current.amount),
      0
    );
    let subtotal = modifiedQuote["additionalPayments-amount"] - total;
    modifiedQuote["additionalPayments-amount"] = subtotal;
    modifiedQuote["total-manual-additions"] = total;
    const monthlyAmount = +(
      modifiedQuote["additionalPayments-amount"] / modifiedQuote.monthlyPayments
    ).toFixed(2);
    for (var i = 0; i < quote.monthlyPayments; i++) {
      payments.push({
        id: uuidv4(),
        amount: monthlyAmount,
        date: moment()
          .add(i + 1, "M")
          .format("yyyy-MM-DD"),
      });
    }
    modifiedQuote["additions-payments"] = payments;
    dispatch({ type: "REPLACE_QUOTE", payload: modifiedQuote });
  };

  const handleRemoveManualPayment = (id) => {
    const modifiedQuote = { ...quote };
    const manualOp = quote["manual-additions"].find((x) => x.id === id);
    modifiedQuote["manual-additions"] = [
      ...modifiedQuote["manual-additions"],
    ].filter((x) => x.id !== id);

    let total = 0;
    let payments = [...modifiedQuote["additions-payments"]];
    total += modifiedQuote["manual-additions"].reduce(
      (accumulated, current) => (accumulated += current.amount),
      0
    );
    let subtotal = modifiedQuote["additionalPayments-amount"] + manualOp.amount;
    modifiedQuote["additionalPayments-amount"] = subtotal;
    modifiedQuote["total-manual-additions"] = total;
    const monthlyAmount = +(
      modifiedQuote["additionalPayments-amount"] / modifiedQuote.monthlyPayments
    ).toFixed(2);
    payments.forEach((p) => {
      p.amount = monthlyAmount;
    });
    modifiedQuote["additions-payments"] = payments;
    dispatch({ type: "REPLACE_QUOTE", payload: modifiedQuote });
  };

  useEffect(() => {
    calculateMontlyPayments();
  }, []);

  return (
    <Wrapper style={{ flexFlow: "column", overflow: "unset" }}>
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "1fr 1fr",
          gridGap: "1em",
        }}
      >
        <QuoteDetailsBox
          value={quote.unit.displayName}
          height="60px"
          title={"Nombre"}
          fontSize={22}
          isText
        />
        <QuoteDetailsBox
          value={quote["additionalPayments-amount"]}
          height="60px"
          title={"Por diferir"}
          fontSize={22}
        />
      </div>

      <StyledWrapper>
        <span>
          <h3 style={{ marginBottom: 0, marginTop: 0, fontWeight: "normal" }}>
            Cantidad estimada
          </h3>
          <StyledSeparator />
        </span>
        <span>
          <h3 style={{ marginBottom: 0, marginTop: 0, fontWeight: "normal" }}>
            Fecha estimada
          </h3>
          <StyledSeparator />
        </span>
        <span></span>

        <React.Fragment>
          {quote["additions-payments"].map((q) => (
            <React.Fragment key={q.id}>
              <TextField
                style={{
                  gridArea: "header",
                }}
                value={q.amount}
                autoComplete={"off"}
                fullWidth
                color="secondary"
                required
                disabled
                onChange={(e) => handleTextChange(e, q.id)}
                name="amount"
                label="Cantidad"
                variant="outlined"
                stylee={{
                  labelRoot: {
                    fontSize: '30px !important'
                  }
                }}
                InputProps={{
                  autoComplete: "off",
                  inputComponent: NumberFormatCustom,
                }}
              />

              <TextField
                style={{
                  gridArea: "header",
                }}
                value={q.date}
                fullWidth
                color="secondary"
                required
                onChange={(e) => {
                  handleDateChange(e, q.id);
                }}
                name="date"
                label="Fecha"
                type="date"
                variant="outlined"
                styleee={{
                  labelRoot: {
                    fontSize: '30px !important'
                  }
                }}
                InputLabelProps={{
                  shrink: true,
                }}
                InputProps={{ inputProps: { min: minDate } }}
              />
              <span></span>
            </React.Fragment>
          ))}

          <React.Fragment>
            {quote["manual-additions"].map((q) => (
              <React.Fragment key={q.id}>
                <StyledManualOperationWrapper>
                  {" "}
                  {MONEYFORMATTER.format(q.amount)}{" "}
                </StyledManualOperationWrapper>
                <StyledManualOperationWrapper>
                  {" "}
                  {q.date}{" "}
                </StyledManualOperationWrapper>
                <div style={{ display: "flex", alignSelf: "end" }}>
                  <InverlevyButton
                    secondary
                    onClick={() => {
                      handleRemoveManualPayment(q.id);
                    }}
                  >
                    Quitar
                  </InvyButton>
                </div>
              </React.Fragment>
            ))}
          </React.Fragment>

          <div>
            <TextField
              value={operation.amount}
              autoComplete={"off"}
              fullWidth
              color="secondary"
              required
              onChange={(e) => {
                const value = parseFloat(e.target.value);
                const top = quote["additionalPayments-amount"];
                if (value > top) {
                  e.target.value = operation.amount;
                  return;
                }
                setOperation({ ...operation, amount: value });
              }}
              id="amount"
              name="amount"
              label="Cantidad"
              variant="outlined"
              style={{
                labelRoot: {
                  fontSize: "30px !important",
                },
              }}
              InputProps={{
                autoComplete: "off",
                inputComponent: NumberFormatCustom,
              }}
            />
          </div>
          <div>
            <TextField
              value={operation.date}
              fullWidth
              color="secondary"
              required
              onChange={(e) => {
                setOperation({ ...operation, date: e.target.value });
              }}
              id="date"
              name="date"
              label="Fecha"
              type="date"
              variant="outlined"
              InputLabelProps={{
                shrink: true,
              }}
              InputProps={{ inputProps: { min: minDate } }}
              style={{
                labelRoot: {
                  fontSize: "30px !important",
                },
              }}
            />
          </div>
          <div style={{ display: "flex", alignSelf: "end" }}>
            <InvyButton
              disabled={operation.date === "" || operation.amount === ""}
              onClick={handleClick}
            >
              Añadir
            </InvyButton>
          </div>
        </React.Fragment>
      </StyledWrapper>
    </Wrapper>
  );
};

export default FixedPayments;

FixedPayments.propTypes = {
  state: PropTypes.object.isRequired,
  dispatch: PropTypes.func.isRequired,
};
