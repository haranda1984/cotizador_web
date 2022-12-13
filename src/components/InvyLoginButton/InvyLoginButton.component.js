import styled, { css } from 'styled-components'
import { PRIMARYCOLOR } from '../../constants/utils'

const secondaryColor = '#F2F2F2'
export const InverlevyLoginButton = styled.button(props =>
  css`
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 16px;
    border-radius: 100px;
    margin: 1em auto;
    padding: 12px 12px;
    font-weight: 500;
    border: solid 1px ${PRIMARYCOLOR};
    border-color: ${PRIMARYCOLOR};
    font-family: 'Open Sans', sans-serif;
    background-color: ${props.secondary ? secondaryColor : PRIMARYCOLOR};
    color: ${props.secondary ? '#000000' : 'white'};
    box-shadow: '0 2px 0 0 transparent' : '0 2px 0 0 #27b0b0';
    position: relative;
    width: 20em;

    &:hover {
      cursor: pointer;
    }

    &:disabled {
      background-color: #cccccc;
      border: 1px solid #cccccc;
      cursor: not-allowed;

      box-shadow: 0 2px 0 0 #9e9e9e;
    }
  `
)
