import styled, { css } from 'styled-components'

const primaryColor = '#015cb2'
const secondaryColor = '#F2F2F2'

export const InvyButton = styled.button(props =>
  css`
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 16px;
    border-radius: 5px;
    padding: 12px 12px;
    font-weight: 500;
    align-self: ${props.alignSelf ? props.alignSelf : ' '};
    border: solid 1px ${props.borderColor ? props.borderColor : primaryColor};
    border-color: ${props.borderColor ? props.borderColor : primaryColor};
    font-family: 'Open Sans', sans-serif;
    background-color: ${props.backgroundColor ? props.backgroundColor : props.secondary ? secondaryColor : primaryColor};
    color: ${props.color ? props.color : props.secondary ? primaryColor : 'white'};
    box-shadow: ${props.shadow ? '0 10px 10px -2px rgb(0 0 0 / 25%)' : ''};
    position: relative;

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
