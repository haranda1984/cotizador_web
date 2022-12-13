import React, { memo } from "react";
import styled from "styled-components";

import { PRIMARYCOLOR } from "../../constants/utils";

import { InverlevyButton } from "../../components/InvyButton/InvyButton.component";
import { NativeSelect } from "@material-ui/core";

const HeaderContainer = styled.div`
  display: flex;
  flex-flow: row;
  align-items: center;
  color: white;
  width: 70%;
  justify-content: space-between;
  background-color: ${PRIMARYCOLOR};
`;
// eslint-disable-next-line react/prop-types
const QuoteHeader = memo(
  ({ title, onNext, onPrevious, disabled, lastStep, state, dispatch }) => {
      
    const handleCurrencyChange = (e) => {
      dispatch({ type: "SET_CURRENCY", payload: e.target.value });
    };

    return (
      <HeaderContainer>
        <h2 style={{ marginLeft: "2em" }}>{title}</h2>
        <div style={{ display: "flex", marginRight: "2em" }}>
          <NativeSelect
            onChange={handleCurrencyChange}
            disabled={state.step !== 1}
            value={state.currency}
            name="currency"
            label="Moneda"
            style={{ margin: "1em 0px", width: "7em", color: "inherit" }}
            inputProps={{
              name: "currency",
              id: "currency",
            }}
          >
            <option value="MXN">MXN</option>
            {/*<option value="USD">USD</option>*/}
          </NativeSelect>
          {onPrevious && <div style={{ marginRight: "1em" }} />}
          {onPrevious && (
            <InverlevyButton
              secondary
              alignSelf={"center"}
              onClick={onPrevious}
            >
              Anterior
            </InverlevyButton>
          )}
          <div style={{ marginRight: "1em" }} />
          <InverlevyButton
            shadow
            alignSelf={"center"}
            borderColor={"white"}
            onClick={onNext}
            disabled={disabled}
          >
            {lastStep ? "Cotizar" : "Siguiente"}
          </InverlevyButton>
        </div>
      </HeaderContainer>
    );
  }
);

export default QuoteHeader;
